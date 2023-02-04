using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Fence;
using fence_backend.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace fence_backend.Services
{
    public class GrpcFenceService : FenceManager.FenceManagerBase
    {
        public GrpcFenceService( ConfigService configService, MouseHookService mouseHookService )
        {
            mConfigService = configService;
            mMouseHookService = mouseHookService;
        }

        public override Task<ConfigResponse> GetConfig( Empty request, ServerCallContext context )
        {
            var response = new ConfigResponse();

            foreach( var configMonitor in mConfigService.Config.Monitors )
            {
                Console.WriteLine( configMonitor.Left );
            }

            response.Monitors.AddRange( mConfigService.Config.Monitors.Select( monitor => monitor.ToProtoMonitor() ) );

            return Task.FromResult( response );
        }

        public override async Task GetCursorLocationStream( Empty request,
            IServerStreamWriter<CursorLocation> responseStream,
            ServerCallContext context ) =>
            await mMouseHookService
                .MouseEventObservable
                .Do( data =>
                {
                    responseStream.WriteAsync(
                        new CursorLocation { X = data.X, Y = data.Y }
                    );
                } );

        public override async Task GetHeartbeatStream( Empty request,
            IServerStreamWriter<Empty> responseStream,
            ServerCallContext context ) =>
            await Observable
                .Interval( TimeSpan.FromSeconds( 1 ) )
                .Do( _ => responseStream.WriteAsync( new Empty() ) );

        private ConfigService mConfigService;
        private MouseHookService mMouseHookService;
    }

    public class GrpcServerStartup
    {
        public void ConfigureServices( IServiceCollection services )
        {
            var monitorService = new MonitorService();
            var configService = new ConfigService( monitorService );
            var fenceStateService = new FenceStateService();
            var mouseHookService = new MouseHookService( fenceStateService, configService );

            services.AddGrpc();
            services.AddSingleton<FenceStateService>();
            services.AddSingleton( monitorService );
            services.AddSingleton( configService );
            services.AddSingleton( mouseHookService );
        }

        public void Configure( IApplicationBuilder app ) =>
            app.UseRouting().UseEndpoints( endpoints =>
                endpoints.MapGrpcService<GrpcFenceService>()
            );
    }
}