using System;
using System.Text.Json;

namespace MusicStreaming.AdminApp.Models
{
    public class TcpRequest
    {
        public string RequestId { get; set; } = Guid.NewGuid().ToString();
        public string Action { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
    }

    public class TcpResponse
    {
        public string RequestId { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
        public bool IsSuccess => StatusCode >= 200 && StatusCode <= 299;
    }


    public class SingerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class GenreDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

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

    public class SingerAdminDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
    }

    public class CreateSingerDto
    {
        public string Name { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty; 
        public string AvatarFileName { get; set; } = string.Empty;
    }

    public class UpdateSingerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty; 
        public string AvatarFileName { get; set; } = string.Empty;
    }

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

    public class DashboardDataDto
    {
        public UserStatDto UserStats { get; set; } = new();
        public SongStatDto SongStats { get; set; } = new();
        public RevenueStatDto RevenueStats { get; set; } = new();
        public List<PackageStatDto> PackageStats { get; set; } = new();
        public List<TopSongDto> TopListenedSongs { get; set; } = new();
        public List<TopSongDto> TopLikedSongs { get; set; } = new();
    }

    public class UserStatDto
    {
        public int TotalUsers { get; set; }
        public int VipUsers { get; set; }
        public int NormalUsers { get; set; }
    }

    public class SongStatDto
    {
        public int TotalSongs { get; set; }
        public int ActiveSongs { get; set; }
        public int DisabledSongs { get; set; }
    }

    public class RevenueStatDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal CurrentMonthRevenue { get; set; }
        public decimal CurrentYearRevenue { get; set; }
    }

    public class PackageStatDto
    {
        public string PackageName { get; set; } = string.Empty;
        public int UserCount { get; set; }
        public int DurationInMonths { get; set; }
    }

    public class TopSongDto
    {
        public int SongId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SingerName { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class SubscriptionAdminDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationInMonths { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateSubscriptionDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationInMonths { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateSubscriptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationInMonths { get; set; }
        public bool IsActive { get; set; }
    }
}
