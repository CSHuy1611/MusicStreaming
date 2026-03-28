using MediatR;
using MusicStreaming.Application.Features.Genres.Queries.GetAllGenres;
using MusicStreaming.TcpServer.Routing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Routing.Endpoints.Songs
{
    public class GetGenresSongEndpoint : IEndpointHandler
    {
        private readonly IMediator _mediator;

        public GetGenresSongEndpoint(IMediator mediator)
        {
            _mediator = mediator;
        }

        public string ActionName => "GetGenresSong";

        public async Task<object> HandleAsync(string payload, CancellationToken cancellationToken)
        {
            var query = new GetGenresQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return result;
        }
    }
}
