using MediatR;
using MusicStreaming.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Playlists.Queries.GetPlaylistDetail
{
    public class GetPlaylistDetailQuery : IRequest<PlaylistDetailVm>
    {
        public int PlaylistId { get; set; }
        public string? UserId { get; set; }
    }
}
