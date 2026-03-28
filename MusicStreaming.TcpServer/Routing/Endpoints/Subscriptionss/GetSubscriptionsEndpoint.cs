using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.TcpServer.Routing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Routing.Endpoints.Subscriptionss
{
    public class GetSubscriptionsEndpoint : IEndpointHandler
    {
        private readonly ISubscriptionAdminService _subscriptionAdminService;

        public GetSubscriptionsEndpoint(ISubscriptionAdminService subscriptionAdminService)
        {
            _subscriptionAdminService = subscriptionAdminService;
        }

        public string ActionName => "GetSubscriptions";

        public async Task<object> HandleAsync(string payload, CancellationToken cancellationToken)
        {
            return await _subscriptionAdminService.GetAllSubscriptionsAsync(cancellationToken);
        }
    }
}
