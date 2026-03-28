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
    public class CreateSubscriptionEndpoint : IEndpointHandler
    {
        private readonly ISubscriptionAdminService _subscriptionAdminService;

        public CreateSubscriptionEndpoint(ISubscriptionAdminService subscriptionAdminService)
        {
            _subscriptionAdminService = subscriptionAdminService;
        }

        public string ActionName => "CreateSubscription";

        public async Task<object> HandleAsync(string payload, CancellationToken cancellationToken)
        {
            var dto = JsonSerializer.Deserialize<CreateSubscriptionDto>(payload, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (dto == null) throw new ArgumentException("Payload is null or invalid.");

            return await _subscriptionAdminService.CreateSubscriptionAsync(dto, cancellationToken);
        }
    }
}
