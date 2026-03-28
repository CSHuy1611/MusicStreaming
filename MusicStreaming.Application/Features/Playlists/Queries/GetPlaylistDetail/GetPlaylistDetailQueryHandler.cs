using MediatR;
using MusicStreaming.Application.Common.Dtos;
using MusicStreaming.Application.Common.Interfaces.Queries;
using MusicStreaming.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Playlists.Queries.GetPlaylistDetail
{
    public class GetPlaylistDetailQueryHandler : IRequestHandler<GetPlaylistDetailQuery, PlaylistDetailVm>
    {
        private readonly IPlaylistQueries _playlistQueries;

        public GetPlaylistDetailQueryHandler(IPlaylistQueries playlistQueries)
        {
            _playlistQueries = playlistQueries;
        }

        public async Task<PlaylistDetailVm> Handle(GetPlaylistDetailQuery request, CancellationToken cancellationToken)
        {
            return await _playlistQueries.GetPlaylistDetailAsync(request.PlaylistId, request.UserId, cancellationToken);
        }
    }
}
