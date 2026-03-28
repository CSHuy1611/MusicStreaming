using System;

namespace MusicStreaming.Application.Common.Dtos.Admin
{
    public class SongAdminDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
        public bool IsVip { get; set; }
        public int ListenCount { get; set; }
        public string AudioUrl { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int SingerId { get; set; }
        public string SingerName { get; set; } = string.Empty;
        public int GenreId { get; set; }
        public string GenreName { get; set; } = string.Empty;
    }

    public class CreateSongDto
    {
        public string Title { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
        public bool IsVip { get; set; }
        public int SingerId { get; set; }
        public int GenreId { get; set; }
        public string? AudioUrl { get; set; }
        public string AudioFileName { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
        public string ImageFileName { get; set; } = string.Empty;
    }

    public class UpdateSongDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
        public bool IsVip { get; set; }
        public int SingerId { get; set; }
        public int GenreId { get; set; }
        public string? AudioUrl { get; set; }
        public string AudioFileName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string ImageFileName { get; set; } = string.Empty;
    }
}
