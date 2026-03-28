using MusicStreaming.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Domain.Entities
{
    public class Playlist : BaseEntity
    {
        public string? UserId { get; private set; }
        public string Name { get; private set; }
        public bool IsSystemPlaylist { get; private set; }
        public ICollection<PlaylistSong> PlaylistSongs { get; private set; }

        private Playlist()
        {
            Name = string.Empty;
            PlaylistSongs = new List<PlaylistSong>();
        }

        public Playlist(string userId, string name)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User lỗi.");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Tên Playlist trống.");

            UserId = userId;
            Name = name;
            IsSystemPlaylist = false;
            PlaylistSongs = new List<PlaylistSong>();
        }

        public Playlist(string name, bool isSystemPlaylist)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Tên Playlist trống.");

            UserId = null; 
            Name = name;
            IsSystemPlaylist = isSystemPlaylist;
            PlaylistSongs = new List<PlaylistSong>();
        }

        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName)) throw new ArgumentException("Tên mới trống.");
            Name = newName;
            UpdateTime();
        }
    }
}
