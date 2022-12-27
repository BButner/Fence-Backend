namespace fence_backend.Models
{
    public class Monitor
    {
        public double Top { get; set; }
        public double Left { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsSelected { get; set; }
    }
}