using Microsoft.AspNetCore.SignalR;
using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.Application.Services;

namespace MusicStreaming.Web.Hubs
{

    public class DashboardNotifier : IDashboardNotifier
    {
        private readonly IHubContext<DashboardHub> _hubContext;
        private readonly IDashboardService _dashboardService;

        public DashboardNotifier(IHubContext<DashboardHub> hubContext, IDashboardService dashboardService)
        {
            _hubContext = hubContext;
            _dashboardService = dashboardService;
        }

        public async Task BroadcastDashboardUpdateAsync()
        {
            var data = await _dashboardService.GetDashboardDataAsync(); 
            await _hubContext.Clients.All.SendAsync("ReceiveDashboardUpdate", data);
        }
    }
}
