using System.Collections.Generic;

namespace MusicStreaming.Application.Common.Dtos.Admin
{
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
}