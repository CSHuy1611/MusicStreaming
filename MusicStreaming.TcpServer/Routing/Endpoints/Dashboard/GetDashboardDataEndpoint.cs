using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.TcpServer.Routing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Routing.Endpoints.Dashboard
{
    public class GetDashboardDataEndpoint : IEndpointHandler
    {
        private readonly IDashboardService _dashboardService;

        public GetDashboardDataEndpoint(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public string ActionName => "GetDashboardData";

        public async Task<object> HandleAsync(string payload, CancellationToken cancellationToken)
        {
            return await _dashboardService.GetDashboardDataAsync(cancellationToken);
        }
    }
}
