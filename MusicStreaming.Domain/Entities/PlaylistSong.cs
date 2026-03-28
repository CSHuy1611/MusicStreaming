using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Domain.Entities
{
    public class PlaylistSong
    {
        public int PlaylistId { get; private set; }
        public int SongId { get; private set; }
        public DateTime AddedDate { get; private set; }

        public Playlist Playlist { get; private set; }
        public Song Song { get; private set; }

        public PlaylistSong(int playlistId, int songId)
        {
            PlaylistId = playlistId;
            SongId = songId;
            AddedDate = DateTime.UtcNow;
        }
    }
}
