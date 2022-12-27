using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using fence_backend.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace fence_backend
{
    public class Worker : BackgroundService
    {
        public Worker( ILogger<Worker> logger )
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync( CancellationToken stoppingToken ) =>
            await Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults( builder =>
                {
                    builder.ConfigureKestrel( options =>
                        {
                            options.ListenAnyIP( 50052,
                                listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; } );
                        } )
                        .UseKestrel()
                        .UseStartup<GrpcServerStartup>();
                } )
                .Build()
                .StartAsync( stoppingToken );

        private readonly ILogger<Worker> _logger;
    }
}