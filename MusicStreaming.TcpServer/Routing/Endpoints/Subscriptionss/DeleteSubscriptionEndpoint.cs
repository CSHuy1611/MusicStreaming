using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.TcpServer.Routing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Routing.Endpoints.Subscriptionss
{
    public class DeleteSubscriptionEndpoint : IEndpointHandler
    {
        private readonly ISubscriptionAdminService _subscriptionAdminService;

        public DeleteSubscriptionEndpoint(ISubscriptionAdminService subscriptionAdminService)
        {
            _subscriptionAdminService = subscriptionAdminService;
        }

        public string ActionName => "DeleteSubscription";

        public async Task<object> HandleAsync(string payload, CancellationToken cancellationToken)
        {
            if (!int.TryParse(payload, out int id))
            {
                throw new ArgumentException("Invalid ID payload.");
            }

            var result = await _subscriptionAdminService.DeleteSubscriptionAsync(id, cancellationToken);
            if (!result) throw new Exception("Xóa thất bại. Không tìm thấy ID.");

            return result;
        }
    }
}
