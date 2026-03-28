using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Playlists.Commands.AddSongToPlaylist
{
    public class AddSongToPlaylistCommand : IRequest<bool>
    {
        public int PlaylistId { get; set; }
        public int SongId { get; set; }
        public string UserId { get; set; }
    }
}
