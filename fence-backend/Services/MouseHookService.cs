using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Threading;
using fence_backend.Models.WinHook;

namespace fence_backend.Services
{
    public class MouseHookService : IHookListener
    {
        public MouseHookService( FenceStateService fenceStateService, ConfigService configService )
        {
            mDelegate = new HookUtil.HookProc( OnMouseEventReceived );

            mFenceStateService = fenceStateService;
            mConfigService = configService;

            var thread = new Thread( Start );
            thread.IsBackground = true;
            thread.Start();
        }

        public void Dispose() => Stop();

        public void Start()
        {
            mHandle = HookUtil.SetWindowsHookEx( HookType.WH_MOUSE_LL, OnMouseEventReceived, IntPtr.Zero, 0 );

            while( true )
            {
                int result = HookUtil.GetMessage( out var msg, IntPtr.Zero, 0, 0 );

                if( result <= 0 )
                {
                    break;
                }
            }
        }

        public void Stop()
        {
            if( mHandle.HasValue ) HookUtil.UnhookWindowsHookEx( mHandle.Value );
            mHandle = null;
        }

        public IObservable<Point> MouseEventObservable => mSubject;

        private IntPtr OnMouseEventReceived( int code, IntPtr wParam, IntPtr lParam )
        {
            if( code < 0 || wParam != (IntPtr)WM_MOUSEMOVE )
                return (IntPtr)HookUtil.CallNextHookEx( IntPtr.Zero, code, wParam, lParam );

            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure( lParam, typeof( MSLLHOOKSTRUCT ) );

            var point = new Point( hookStruct.pt.x, hookStruct.pt.y );

            mSubject.OnNext( point );

            if( !mFenceStateService.IsActive )
            {
                return (IntPtr)HookUtil.CallNextHookEx( IntPtr.Zero, code, wParam, lParam );
            }

            bool inside = false;

            foreach( var m in mConfigService.Config.Monitors.Where( monitor => monitor.IsSelected ) )
            {
                if( !( point.X >= m.Left + 1 )
                    || !( point.X <= m.Left + ( m.Width - 1 ) )
                    || !( point.Y >= m.Top + 1 )
                    || !( point.Y <= m.Top + ( m.Height - 1 ) ) ) continue;

                inside = true;
                break;
            }

            if( inside )
            {
                mLastGoodX = point.X;
                mLastGoodY = point.Y;
                return (IntPtr)HookUtil.CallNextHookEx( IntPtr.Zero, code, wParam, lParam );
            }
            else
            {
                var monitor = mConfigService.Config.Monitors.FirstOrDefault( mon => mLastGoodX >= mon.Left
                    && mLastGoodX <= mon.Left + mon.Width
                    && mLastGoodY >= mon.Top
                    && mLastGoodY <= mon.Top + mon.Height );

                if( monitor is not null )
                {
                    double newX = point.X;
                    double newY = point.Y;

                    if( point.X < monitor.Left )
                    {
                        newX = monitor.Left;
                    }
                    else if( point.X > monitor.Left + monitor.Width )
                    {
                        newX = monitor.Left + monitor.Width;
                    }

                    if( point.Y < monitor.Top )
                    {
                        newY = monitor.Top;
                    }
                    else if( point.Y > monitor.Top + monitor.Height )
                    {
                        newY = monitor.Top + monitor.Height;
                    }

                    HookUtil.SetCursorPos( (int)newX, (int)newY );
                }
                else
                {
                    HookUtil.SetCursorPos( (int)mLastGoodX, (int)mLastGoodY );
                }
            }

            return (IntPtr)1;
        }

        private const uint WM_MOUSEMOVE = 0x0200;

        private double mLastGoodX = 0;
        private double mLastGoodY = 0;

        private IntPtr? mHandle;
        private readonly HookUtil.HookProc mDelegate;
        private Subject<Point> mSubject = new();
        private FenceStateService mFenceStateService;
        private ConfigService mConfigService;
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }
}