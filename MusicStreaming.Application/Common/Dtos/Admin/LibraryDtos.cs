using System;

namespace MusicStreaming.Application.Common.Dtos.Admin
{
    public class GenreAdminDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class CreateGenreDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateGenreDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class PlaylistAdminDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SongCount { get; set; }
    }

    public class CreateSystemPlaylistDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateSystemPlaylistDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class PlaylistSongDto
    {
        public int SongId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SingerName { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
        public DateTime AddedDate { get; set; }
    }

    public class AddSongToPlaylistDto
    {
        public int PlaylistId { get; set; }
        public int SongId { get; set; }
    }

    public class RemoveSongFromPlaylistDto
    {
        public int PlaylistId { get; set; }
        public int SongId { get; set; }
    }
}