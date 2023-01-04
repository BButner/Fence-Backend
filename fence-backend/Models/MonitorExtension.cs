namespace fence_backend.Models
{
    public static class MonitorExtension
    {
        public static Fence.Monitor ToProtoMonitor( this Monitor monitor ) =>
            new()
                {
                Width = monitor.Width,
                Height = monitor.Height,
                Left = monitor.Left,
                Top = monitor.Top,
                Selected = monitor.IsSelected
                };
    }
}