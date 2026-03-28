using MusicStreaming.Application.Common.Dtos.Admin;
using MusicStreaming.Application.Common.Interfaces.Queries;
using MusicStreaming.Application.Common.Interfaces.Services;
using System.Threading;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardQueries _dashboardQueries;

        public DashboardService(IDashboardQueries dashboardQueries)
        {
            _dashboardQueries = dashboardQueries;
        }

        public async Task<DashboardDataDto> GetDashboardDataAsync(CancellationToken cancellationToken = default)
        {
            return await _dashboardQueries.GetDashboardDataAsync(cancellationToken);
        }
    }
}