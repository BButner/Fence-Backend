using System;
using System.Collections.Generic;
using System.Linq;
using fence_backend.Models;
using MonitorDetails;
using MonitorDetails.Interfaces;

namespace fence_backend.Services
{
    public class MonitorService
    {
        public MonitorService()
        {
            IReader reader = new Reader();
            Monitors = reader.GetMonitorDetails().Select( monitor => new Monitor
                {
                Top = monitor.MonitorCoordinates.Y,
                Left = monitor.MonitorCoordinates.X,
                Width = monitor.Resolution.Width,
                Height = monitor.Resolution.Height,
                IsPrimary = monitor.IsPrimaryMonitor,
                } );

            foreach( var monitor in Monitors )
            {
                Console.WriteLine(
                    $"Added Monitor with Top: {monitor.Top}, Left: {monitor.Left}, Width: {monitor.Width}, Height: {monitor.Height}, IsPrimary: {monitor.IsPrimary}" );
            }
        }

        public bool ValidateMonitors( IEnumerable<Monitor> monitors )
        {
            foreach( var monitor in monitors )
            {
                var monitorDetail = Monitors.FirstOrDefault( m =>
                    m.Top == monitor.Top && m.Left == monitor.Left );
                if( monitorDetail == null )
                {
                    return false;
                }

                if( monitorDetail.Width != monitor.Width
                    || monitorDetail.Height != monitor.Height )
                {
                    return false;
                }

                if( monitorDetail.Left != monitor.Left
                    || monitorDetail.Top != monitor.Top )
                {
                    return false;
                }
            }

            return true;
        }

        public IEnumerable<Monitor> Monitors { get; private set; }
    }
}