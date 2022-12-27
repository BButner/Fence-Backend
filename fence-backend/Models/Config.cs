using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace fence_backend.Models
{
    public class Config
    {
        public void Save() =>
            File.WriteAllText( "config.json",
                JsonSerializer.Serialize( this,
                    new JsonSerializerOptions { WriteIndented = true } ) );

        public bool LockOnStartup { get; set; }
        public IEnumerable<Monitor> Monitors { get; set; }
    }
}