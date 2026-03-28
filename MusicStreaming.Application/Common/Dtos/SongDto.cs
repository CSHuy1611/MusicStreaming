using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Dtos
{
    public class SongDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ArtistName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string AudioUrl { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
        public bool IsVip { get; set; }

        // Helper để hiển thị thời gian dạng mm:ss (VD: 04:05)
        public string DurationFormatted => $"{(int)Duration.TotalMinutes}:{Duration.Seconds:D2}";
    }

}
