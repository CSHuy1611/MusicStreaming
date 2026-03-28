using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Domain.Entities
{
    public class FavoriteSong
    {
        public string UserId { get; private set; }
        public int SongId { get; private set; }
        public DateTime LikedDate { get; private set; }

        public Song Song { get; private set; }
        private FavoriteSong() { }

        public FavoriteSong(string userId, int songId)
        {
            UserId = userId;
            SongId = songId;
            LikedDate = DateTime.UtcNow;
        }
    }
}
