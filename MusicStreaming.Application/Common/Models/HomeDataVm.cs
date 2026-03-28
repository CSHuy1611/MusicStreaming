using MusicStreaming.Application.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Models
{
    public class HomeDataVm
    {
        public IEnumerable<SongDto> TrendingSongs { get; set; }
        public IEnumerable<PlaylistDto> NewPlaylists { get; set; }
        public IEnumerable<ArtistDto> SuggestionArtists { get; set; }

        public bool IsUserVip { get; set; } = false;
    }
}
