using MusicStreaming.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Domain.Entities
{
    public class Genre : BaseEntity
    {
        public string Name { get; private set; }


        public ICollection<Song> Songs { get; private set; }

        public Genre(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên thể loại không được để trống.");

            Name = name;
        }

        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Tên thể loại mới không hợp lệ.");

            Name = newName;
            UpdateTime();
        }
    }
}
