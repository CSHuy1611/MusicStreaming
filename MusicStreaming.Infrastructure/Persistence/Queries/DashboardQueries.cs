using Microsoft.EntityFrameworkCore;
using MusicStreaming.Application.Common.Dtos.Admin;
using MusicStreaming.Application.Common.Interfaces.Queries;
using MusicStreaming.Application.Common.Interfaces;
using MusicStreaming.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MusicStreaming.Infrastructure.Persistence.Queries
{
    public class DashboardQueries : IDashboardQueries
    {
        private readonly ApplicationDbContext _context;
        private readonly IIdentityService _identityService;

        public DashboardQueries(ApplicationDbContext context, IIdentityService identityService)
        {
            _context = context;
            _identityService = identityService;
        }

        public async Task<DashboardDataDto> GetDashboardDataAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            // ==========================================
            // 1. THỐNG KÊ USER (Identity)
            // ==========================================
            int totalUsers = await _identityService.GetTotalUsersCountAsync();
            int vipUsers = await _identityService.GetTotalVipUsersCountAsync();
            int normalUsers = totalUsers - vipUsers;

            // ==========================================
            // 2. THỐNG KÊ BÀI HÁT
            // ==========================================
            // IgnoreQueryFilters() để đếm cả bài bị xóa mềm
            var allSongsQuery = _context.Set<Song>().AsNoTracking().IgnoreQueryFilters();

            int totalSongs = await allSongsQuery.CountAsync(cancellationToken);
            int disabledSongs = await allSongsQuery.CountAsync(s => s.IsDeleted, cancellationToken);
            int activeSongs = totalSongs - disabledSongs;

            // ==========================================
            // 3. THỐNG KÊ DOANH THU
            // ==========================================
            var ordersQuery = _context.Set<Order>().AsNoTracking();

            decimal totalRevenue = await ordersQuery.SumAsync(o => (decimal?)o.Amount, cancellationToken) ?? 0;

            decimal currentMonthRevenue = await ordersQuery
                .Where(o => o.PurchaseDate.Year == now.Year && o.PurchaseDate.Month == now.Month)
                .SumAsync(o => (decimal?)o.Amount, cancellationToken) ?? 0;

            decimal currentYearRevenue = await ordersQuery
                .Where(o => o.PurchaseDate.Year == now.Year)
                .SumAsync(o => (decimal?)o.Amount, cancellationToken) ?? 0;

            // ==========================================
            // 4. THỐNG KÊ GÓI VIP PHỔ BIẾN
            // ==========================================
            var packageStats = await ordersQuery
                .GroupBy(o => new { o.Package.Name, o.Package.DurationInMonths })
                .Select(g => new PackageStatDto
                {
                    PackageName = g.Key.Name,
                    DurationInMonths = g.Key.DurationInMonths,
                    UserCount = g.Count()
                })
                .OrderByDescending(x => x.UserCount)
                .ToListAsync(cancellationToken);

            // ==========================================
            // 5. TOP 10 BÀI HÁT NGHE NHIỀU NHẤT
            // ==========================================
            var topListened = await _context.Set<Song>().AsNoTracking()
                .OrderByDescending(s => s.ListenCount)
                .Take(10)
                .Select(s => new TopSongDto
                {
                    SongId = s.Id,
                    Title = s.Title,
                    SingerName = s.Singer.Name,
                    Count = s.ListenCount
                })
                .ToListAsync(cancellationToken);

            // ==========================================
            // 6. TOP 10 BÀI HÁT THÍCH NHIỀU NHẤT
            // ==========================================
            var topLiked = await _context.Set<FavoriteSong>().AsNoTracking()
                .GroupBy(f => new { f.Song.Id, f.Song.Title, SingerName = f.Song.Singer.Name })
                .Select(g => new TopSongDto
                {
                    SongId = g.Key.Id,
                    Title = g.Key.Title,
                    SingerName = g.Key.SingerName,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToListAsync(cancellationToken);

            // Đóng gói toàn bộ trả về
            return new DashboardDataDto
            {
                UserStats = new UserStatDto { TotalUsers = totalUsers, VipUsers = vipUsers, NormalUsers = normalUsers },
                SongStats = new SongStatDto { TotalSongs = totalSongs, ActiveSongs = activeSongs, DisabledSongs = disabledSongs },
                RevenueStats = new RevenueStatDto { TotalRevenue = totalRevenue, CurrentMonthRevenue = currentMonthRevenue, CurrentYearRevenue = currentYearRevenue },
                PackageStats = packageStats,
                TopListenedSongs = topListened,
                TopLikedSongs = topLiked
            };
        }
    }
}
