using MediatR;
using MusicStreaming.Application.Features.Singers.Queries.GetSingers;
using MusicStreaming.TcpServer.Routing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Routing.Endpoints.Songs
{
    public class GetSingersSongEndpoint : IEndpointHandler
    {
        private readonly IMediator _mediator;

        public GetSingersSongEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public string ActionName => "GetSingersSong";

        public async Task<object> HandleAsync(string payload, CancellationToken cancellationToken)
        {

            var query = new GetSingersQuery();

            var result = await _mediator.Send(query, cancellationToken);

            return result;
        }
    }
}
