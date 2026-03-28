using MusicStreaming.Application.Common.Dtos.Admin;
using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.TcpServer.Routing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Routing.Endpoints.Subscriptionss
{
    public class UpdateSubscriptionEndpoint : IEndpointHandler
    {
        private readonly ISubscriptionAdminService _subscriptionAdminService;

        public UpdateSubscriptionEndpoint(ISubscriptionAdminService subscriptionAdminService)
        {
            _subscriptionAdminService = subscriptionAdminService;
        }

        public string ActionName => "UpdateSubscription";

        public async Task<object> HandleAsync(string payload, CancellationToken cancellationToken)
        {
            var dto = JsonSerializer.Deserialize<UpdateSubscriptionDto>(payload, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (dto == null) throw new ArgumentException("Payload is null or invalid.");

            return await _subscriptionAdminService.UpdateSubscriptionAsync(dto, cancellationToken);
        }
    }
}
