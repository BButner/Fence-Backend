using System;
using System.Runtime.InteropServices;
using fence_backend.Services;

namespace fence_backend.Models.WinHook
{
    internal class HookUtil
    {
        [DllImport( "user32.dll", SetLastError = true )]
        internal static extern bool UnhookWindowsHookEx( IntPtr hhk );

        [DllImport( "user32.dll", SetLastError = true )]
        internal static extern IntPtr
            SetWindowsHookEx( HookType hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId );

        [DllImport( "user32.dll" )]
        internal static extern int CallNextHookEx( IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam );

        [DllImport( "user32.dll" )]
        internal static extern int GetMessage( out MSG lpMsg,
            IntPtr hWnd,
            uint wMsgFilterMin,
            uint wMsgFilterMax );

        [DllImport( "user32.dll" )]
        internal static extern bool SetCursorPos( int x, int y );

        [StructLayout( LayoutKind.Sequential )]
        public struct MSG
        {
            IntPtr hwnd;
            uint message;
            UIntPtr wParam;
            IntPtr lParam;
            int time;
            POINT pt;
            int lPrivate;
        }


        internal delegate IntPtr HookProc( int code, IntPtr wParam, IntPtr lParam );
    }
}