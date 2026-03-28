using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Playlists.Commands.CreatePlaylist
{
    public class CreatePlaylistCommand : IRequest<int>
    {
        public string UserId { get; set; }
        public string Name { get; set; }
    }
}
