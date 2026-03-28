using MusicStreaming.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Domain.Entities
{
    public class Singer : BaseEntity
    {
        public string Name { get; private set; }
        public string AvatarUrl { get; private set; }
        public string Bio { get; private set; }
        public ICollection<Song> Songs { get; private set; }

        private Singer()
        {
            Songs = new List<Song>();
        }

        public Singer(string name, string avatarUrl, string bio)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên ca sĩ không được để trống.");

            Name = name;
            AvatarUrl = avatarUrl;
            Bio = bio;
            Songs = new List<Song>();
        }

        public void UpdateInfo(string name, string avatarUrl, string bio)
        {
            if (!string.IsNullOrEmpty(name)) Name = name;
            if (!string.IsNullOrEmpty(avatarUrl)) AvatarUrl = avatarUrl;
            if (!string.IsNullOrEmpty(bio)) Bio = bio;

            UpdateTime();
        }
    }
}
