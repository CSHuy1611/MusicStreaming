using MediatR;
using MusicStreaming.Application.Common.Dtos;
using MusicStreaming.Application.Common.Interfaces.Queries;
using MusicStreaming.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Songs.Queries.GetAllSongs
{
    public class GetAllSongsQueryHandler : IRequestHandler<GetAllSongsQuery, PagedResult<SongDto>>
    {
        private readonly ISongQueries _songQueries;

        public GetAllSongsQueryHandler(ISongQueries songQueries)
        {
            _songQueries = songQueries;
        }

        public async Task<PagedResult<SongDto>> Handle(GetAllSongsQuery request, CancellationToken cancellationToken)
        {
            return await _songQueries.GetAllSongsAsync(request.PageNumber, request.PageSize, cancellationToken);
        }
    }
}
