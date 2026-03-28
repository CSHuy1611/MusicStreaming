using MusicStreaming.TcpServer.Routing.Interfaces;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Routing.Endpoints.Songs
{
    public class TestConnectionEndpoint : IEndpointHandler
    {
        public string ActionName => "TestConnection";

        public Task<object> HandleAsync(string payload, CancellationToken cancellationToken)
        {
            // Simulate processing
            var responseData = new
            {
                ServerTime = DateTime.UtcNow,
                Message = "TCP Server is running nicely!",
                ReceivedPayload = payload
            };

            return Task.FromResult<object>(responseData);
        }
    }
}
