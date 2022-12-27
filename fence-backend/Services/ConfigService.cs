using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using fence_backend.Models;

namespace fence_backend.Services
{
    public class ConfigService
    {
        public ConfigService( MonitorService monitorService )
        {
            if( File.Exists( "config.json" ) )
            {
                string jsonString = File.ReadAllText( "config.json" );
                Config = JsonSerializer.Deserialize<Config>( jsonString );

                if( !monitorService.ValidateMonitors( Config.Monitors ) )
                {
                    Config.Monitors = monitorService.Monitors;
                    Config.Save();
                }
            }
            else
            {
                var config = new Config { Monitors = monitorService.Monitors };
                config.Save();
                Config = config;
            }
        }

        public Config Config { get; private set; }
    }
}