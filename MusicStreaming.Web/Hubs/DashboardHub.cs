using Microsoft.AspNetCore.SignalR;
using MusicStreaming.Application.Common.Interfaces.Services;

namespace MusicStreaming.Web.Hubs
{
    public class DashboardHub : Hub
    {
        private readonly IDashboardService _dashboardService;

        public DashboardHub(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task TriggerUpdateFromTcp()
        {
            try
            {
                Console.WriteLine(">>> [HUB]: Đã nhận được lệnh Bóp cò!");
                var data = await _dashboardService.GetDashboardDataAsync();
                await Clients.All.SendAsync("ReceiveDashboardUpdate", data);
                Console.WriteLine(">>> [HUB]: Đã đẩy data mới về cho WPF thành công!");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($">>> [HUB LỖI]: {ex.Message} - {ex.InnerException?.Message}");
                Console.ResetColor();
                throw;
            }
        }
    }
}