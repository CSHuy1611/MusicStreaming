using MusicStreaming.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Domain.Entities
{
    public class Song : BaseEntity
    {
        public string Title { get; private set; } 
        public string AudioUrl { get; private set; }
        public string ImageUrl { get; private set; }
        public TimeSpan Duration { get; private set; }
        public bool IsVip { get; private set; }
        public int ListenCount { get; private set; }

        // Foreign Keys
        public int SingerId { get; private set; }
        public int GenreId { get; private set; }

        // Navigations
        public Singer Singer { get; private set; }
        public Genre Genre { get; private set; }

        public Song(string title, string audioUrl, string imageUrl, TimeSpan duration, int singerId, int genreId, bool isVip)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Tên bài hát trống.");
            if (string.IsNullOrWhiteSpace(audioUrl)) throw new ArgumentException("Link nhạc trống.");

            Title = title;
            AudioUrl = audioUrl;
            ImageUrl = imageUrl;
            Duration = duration;
            SingerId = singerId;
            GenreId = genreId;
            IsVip = isVip;
            ListenCount = 0; 
        }


        public void IncrementListenCount()
        {
            ListenCount++;
        }

        public void SetVipStatus(bool isVip)
        {
            IsVip = isVip;
            UpdateTime();
        }

        public void UpdateFullInfo(string title, string audioUrl, string imageUrl, TimeSpan duration, int singerId, int genreId, bool isVip)
        {
            if (!string.IsNullOrWhiteSpace(title)) Title = title;
            if (!string.IsNullOrWhiteSpace(audioUrl)) AudioUrl = audioUrl;
            if (!string.IsNullOrWhiteSpace(imageUrl)) ImageUrl = imageUrl;
            Duration = duration;
            if (singerId > 0) SingerId = singerId;
            if (genreId > 0) GenreId = genreId;
            IsVip = isVip;
            UpdateTime();
        }

    }
}
