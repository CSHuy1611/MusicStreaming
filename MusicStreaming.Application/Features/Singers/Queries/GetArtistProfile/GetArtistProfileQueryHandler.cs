using MediatR;
using MusicStreaming.Application.Common.Interfaces.Queries;
using MusicStreaming.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Singers.Queries.GetArtistProfile
{
    public class GetArtistProfileQueryHandler : IRequestHandler<GetArtistProfileQuery, ArtistProfileVm>
    {
        private readonly ISingerQueries _singerQueries;
        public GetArtistProfileQueryHandler(ISingerQueries singerQueries)
        {
            _singerQueries = singerQueries;
        }

        public async Task<ArtistProfileVm> Handle(GetArtistProfileQuery request, CancellationToken cancellationToken)
        {
            return await _singerQueries.GetArtistProfileAsync(request.SingerId, cancellationToken);
        }
    }
}
