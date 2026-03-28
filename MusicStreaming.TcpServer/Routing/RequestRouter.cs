using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MusicStreaming.Application.Common.Exceptions;
using MusicStreaming.TcpServer.Protocol.Models;
using MusicStreaming.TcpServer.Routing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Routing
{
    public class RequestRouter : IRequestRouter
    {
        private readonly ILogger<RequestRouter> _logger;
        private readonly IServiceProvider _serviceProvider;

        public RequestRouter(ILogger<RequestRouter> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task<TcpResponse> RouteAsync(TcpRequest request, CancellationToken cancellationToken)
        {
            try
            {
               
                var endpoints = _serviceProvider.GetServices<IEndpointHandler>();
                var handler = endpoints.FirstOrDefault(e => e.ActionName.Equals(request.Action, StringComparison.OrdinalIgnoreCase));

                if (handler == null)
                {
                    _logger.LogWarning("Action {Action} not found for RequestId {RequestId}", request.Action, request.RequestId);
                    return TcpResponse.Error(request.RequestId, 404, $"Action '{request.Action}' not found.");
                }

                _logger.LogInformation("Executing action {Action} for RequestId {RequestId}", request.Action, request.RequestId);

                var result = await handler.HandleAsync(request.Payload, cancellationToken);
                
                var responseData = JsonSerializer.Serialize(result);
                _logger.LogInformation("Action {Action} executed successfully for RequestId {RequestId}", request.Action, request.RequestId);

                return TcpResponse.Success(request.RequestId, responseData);
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning("Validation error for {Action}: {Message}", request.Action, ex.Message);
                return TcpResponse.Error(request.RequestId, 400, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while routing request {RequestId} for Action {Action}", request.RequestId, request.Action);
                return TcpResponse.Error(request.RequestId, 500, ex.Message);
            }
        }
    }
}
