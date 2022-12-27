using fence_backend.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MonitorDetails;
using MonitorDetails.Interfaces;

namespace fence_backend
{
    public class Program
    {
        public static void Main( string[] args ) => CreateHostBuilder( args ).Build().Run();

        public static IHostBuilder CreateHostBuilder( string[] args ) =>
            Host.CreateDefaultBuilder( args )
                .ConfigureServices( ( hostContext, services ) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddGrpc();
                } );
    }
}