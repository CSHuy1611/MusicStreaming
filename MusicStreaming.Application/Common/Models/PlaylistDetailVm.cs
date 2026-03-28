using MusicStreaming.Application.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Models
{
    public class PlaylistDetailVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool IsUserVip { get; set; }
        // Danh sách bài hát trong Playlist này
        public IEnumerable<SongDto> Songs { get; set; }
    }
}
