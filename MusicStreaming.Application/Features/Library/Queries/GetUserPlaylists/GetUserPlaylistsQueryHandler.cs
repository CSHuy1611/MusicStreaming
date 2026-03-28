using MediatR;
using MusicStreaming.Application.Common.Dtos;
using MusicStreaming.Application.Common.Interfaces.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Library.Queries.GetUserPlaylists
{
    public class GetUserPlaylistsQueryHandler : IRequestHandler<GetUserPlaylistsQuery, List<LibraryPlaylistDto>>
    {
        private readonly ILibraryQueries _libraryQueries;

        public GetUserPlaylistsQueryHandler(ILibraryQueries libraryQueries)
        {
            _libraryQueries = libraryQueries;
        }

        public async Task<List<LibraryPlaylistDto>> Handle(GetUserPlaylistsQuery request, CancellationToken cancellationToken)
        {
            return await _libraryQueries.GetUserPlaylistsAsync(request.UserId, cancellationToken);
        }
    }
}
