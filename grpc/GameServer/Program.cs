using GameServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GameServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenLocalhost(5000, listenOptions =>
                {
                    listenOptions.Protocols =
                        Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
                });
            });

            builder.Services.AddGrpc();

            var app = builder.Build();

            app.MapGrpcService<GameService>();

            app.Run();
        }
    }
}
