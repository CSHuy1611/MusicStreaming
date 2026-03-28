using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MusicStreaming.Application;
using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.Infrastructure;
using MusicStreaming.TcpServer.Networking;
using MusicStreaming.TcpServer.Networking.Interfaces;
using MusicStreaming.TcpServer.Protocol.Serialization;
using MusicStreaming.TcpServer.Routing;
using MusicStreaming.TcpServer.Routing.Endpoints.Dashboard;
using MusicStreaming.TcpServer.Routing.Endpoints.Genres;
using MusicStreaming.TcpServer.Routing.Endpoints.Playlists;
using MusicStreaming.TcpServer.Routing.Endpoints.Singers;
using MusicStreaming.TcpServer.Routing.Endpoints.Songs;
using MusicStreaming.TcpServer.Routing.Endpoints.Subscriptionss;
using MusicStreaming.TcpServer.Routing.Interfaces;
using MusicStreaming.TcpServer.Services;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Register Application and Infrastructure layers
                    services.AddApplication();
                    services.AddInfrastructure(hostContext.Configuration);

                    // Register TCP Server Core Services
                    services.AddSingleton<IPacketSerializer, JsonPacketSerializer>();
                    services.AddSingleton<IConnectionManager, ConnectionManager>();
                    
                    services.AddSingleton<IRequestRouter, RequestRouter>();

                    // Register TCP Server Hosted Services
                    services.AddHostedService<TcpServerManager>();
                    services.AddHostedService<ConnectionMonitorService>();

                    // Endpoint mapping (Could be scanned via Reflection later)
                    services.AddTransient<IEndpointHandler, TestConnectionEndpoint>();
                    services.AddTransient<IEndpointHandler, GetSongsEndpoint>();
                    services.AddTransient<IEndpointHandler, CreateSongEndpoint>();
                    services.AddTransient<IEndpointHandler, UpdateSongEndpoint>();
                    services.AddTransient<IEndpointHandler, DeleteSongEndpoint>();
                    services.AddTransient<IEndpointHandler, GetSingersSongEndpoint>();
                    services.AddTransient<IEndpointHandler, GetGenresSongEndpoint>();

                    // Endpoint quản lí ca sĩ
                    services.AddScoped<IEndpointHandler, GetSingersEndpoint>();
                    services.AddScoped<IEndpointHandler, CreateSingerEndpoint>();
                    services.AddScoped<IEndpointHandler, UpdateSingerEndpoint>();
                    services.AddScoped<IEndpointHandler, DeleteSingerEndpoint>();

                    // Endpoint quản lí thư viện
                    services.AddScoped<IEndpointHandler, GetGenresLibEndpoint>();
                    services.AddScoped<IEndpointHandler, CreateGenreLibEndpoint>();
                    services.AddScoped<IEndpointHandler, UpdateGenreLibEndpoint>();
                    services.AddScoped<IEndpointHandler, DeleteGenreLibEndpoint>();

                    services.AddScoped<IEndpointHandler, GetSystemPlaylistsEndpoint>();
                    services.AddScoped<IEndpointHandler, CreateSystemPlaylistEndpoint>();
                    services.AddScoped<IEndpointHandler, UpdateSystemPlaylistEndpoint>();
                    services.AddScoped<IEndpointHandler, DeleteSystemPlaylistEndpoint>();

                    services.AddScoped<IEndpointHandler, GetSongsInPlaylistEndpoint>();
                    services.AddScoped<IEndpointHandler, AddSongToPlaylistEndpoint>();
                    services.AddScoped<IEndpointHandler, RemoveSongFromPlaylistEndpoint>();

                    // Endpoint Dashboard
                    services.AddScoped<IEndpointHandler, GetDashboardDataEndpoint>();

                    // Endpoint quản lí gói vip
                    services.AddTransient<IEndpointHandler, GetSubscriptionsEndpoint>();
                    services.AddTransient<IEndpointHandler, CreateSubscriptionEndpoint>();
                    services.AddTransient<IEndpointHandler, UpdateSubscriptionEndpoint>();
                    services.AddTransient<IEndpointHandler, DeleteSubscriptionEndpoint>();

                    // Service SignalR
                    services.AddSingleton<IDashboardNotifier, TcpDashboardNotifier>();

                })
                .Build();

            await host.RunAsync();
        }
    }
}
