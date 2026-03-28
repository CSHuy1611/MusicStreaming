using MediatR;
using MusicStreaming.Application.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Library.Queries.GetUserPlaylists
{
    public class GetUserPlaylistsQuery : IRequest<List<LibraryPlaylistDto>>
    {
        public string UserId { get; set; }
    }
}
