using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicStreaming.Domain.Entities;
using MusicStreaming.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStreaming.Infrastructure.Persistence
{
    public class ApplicationDbContextInitialiser
    {
        private readonly ILogger<ApplicationDbContextInitialiser> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public ApplicationDbContextInitialiser(
            ILogger<ApplicationDbContextInitialiser> logger,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                if (_context.Database.IsSqlServer())
                {
                    await _context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi khởi tạo Database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi Seed Data.");
                throw;
            }
        }

        public async Task TrySeedAsync()
        {
            // ============================================================
            // 1. SEED ROLES (QUYỀN HẠN)
            // ============================================================
            var roles = new[] { "Admin", "Customer" };
            foreach (var role in roles)
            {
                if (_roleManager.Roles.All(r => r.Name != role))
                {
                    var newRole = new ApplicationRole(role);
                    if (role == "Admin")
                        newRole.Description = "Quản trị viên toàn quyền hệ thống";
                    else
                        newRole.Description = "Người dùng khách hàng, có thể nghe nhạc và mua gói";

                    await _roleManager.CreateAsync(newRole);
                }
            }

            // ============================================================
            // 2. SEED USERS (NGƯỜI DÙNG)
            // ============================================================

            // 2.1 Admin User
            var adminEmail = "admin@system.com";
            if (_userManager.Users.All(u => u.UserName != adminEmail))
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true
                };
                await _userManager.CreateAsync(admin, "Admin@123");
                await _userManager.AddToRoleAsync(admin, "Admin");
            }

            // 2.2 Customer Users
            var customers = new[]
            {
                new { Email = "an.nguyen@test.com", Name = "Nguyễn Văn An", IsVip = true },
                new { Email = "binh.le@test.com", Name = "Lê Thị Bình", IsVip = false },
                new { Email = "cuong.tran@test.com", Name = "Trần Quốc Cường", IsVip = true },
                new { Email = "dung.pham@test.com", Name = "Phạm Thùy Dung", IsVip = false },
                new { Email = "em.hoang@test.com", Name = "Hoàng Văn Em", IsVip = false }
            };

            foreach (var c in customers)
            {
                if (_userManager.Users.All(u => u.UserName != c.Email))
                {
                    var user = new ApplicationUser
                    {
                        UserName = c.Email,
                        Email = c.Email,
                        FullName = c.Name,
                        EmailConfirmed = true
                    };

                    if (c.IsVip)
                    {
                        user.ExtendPremium(12);
                    }

                    await _userManager.CreateAsync(user, "User@123");
                    await _userManager.AddToRoleAsync(user, "Customer");
                }
            }

            // ============================================================
            // 3. SEED CATALOG (NHẠC)
            // ============================================================
            if (!_context.Genres.Any())
            {
                // Genres
                var pop = new Genre("Pop Ballad");
                var rap = new Genre("Rap / Hip-hop");
                var edm = new Genre("Electronic / EDM");
                var indie = new Genre("Indie Việt");

                _context.Genres.AddRange(pop, rap, edm, indie);
                await _context.SaveChangesAsync(); 

                // Singers
                var stmp = new Singer("Sơn Tùng M-TP", "/singers/son-tung-mtp-avatar.jpg", "Hoàng tử mưa");
                var den = new Singer("Đen Vâu", "/singers/den-vau-avatar.jpg", "Rapper tử tế");
                var amee = new Singer("AMEE", "/singers/amee-avatar.jpg", "Công chúa kẹo ngọt");
                var justatee = new Singer("JustaTee", "/singers/justatee-avatar.jpg", "Ông hoàng Melody");
                var binz = new Singer("Binz", "/singers/binz-avatar.jpg", "Bad boy");

                _context.Singers.AddRange(stmp, den, amee, justatee, binz);
                await _context.SaveChangesAsync(); 

                // Songs
                var songs = new List<Song>
                {
                    // Sơn Tùng
                    new Song("Chúng Ta Của Hiện Tại", "/songs/chung-ta-cua-hien-tai.mp3", "/images/chung-ta-cua-hien-tai.jpg", TimeSpan.FromMinutes(5.02), stmp.Id, pop.Id, true),
                    new Song("Muộn Rồi Mà Sao Còn", "/songs/muon-roi-ma-sao-con.mp3", "/images/muon-roi-ma-sao-con.jpg", TimeSpan.FromMinutes(4.48), stmp.Id, pop.Id, false),
                    new Song("Có Chắc Yêu Là Đây", "/songs/co-chac-yeu-la-day.mp3", "/images/co-chac-yeu-la-day.jpg", TimeSpan.FromMinutes(3.35), stmp.Id, pop.Id, false),

                    // Đen Vâu
                    new Song("Mang Tiền Về Cho Mẹ", "/songs/mang-tien-ve-cho-me.mp3", "/images/mang-tien-ve-cho-me.jpg", TimeSpan.FromMinutes(6.41), den.Id, rap.Id, false),
                    new Song("Trốn Tìm", "/songs/tron-tim.mp3", "/images/tron-tim.jpg", TimeSpan.FromMinutes(4.12), den.Id, rap.Id, true),
                    new Song("Hai Triệu Năm", "/songs/hai-trieu-nam.mp3", "/images/hai-trieu-nam.jpg", TimeSpan.FromMinutes(3.37), den.Id, rap.Id, false),

                    // AMEE
                    new Song("Anh Nhà Ở Đâu Thế", "/songs/anh-nha-o-dau-the.mp3", "/images/anh-nha-o-dau-the.jpg", TimeSpan.FromMinutes(4.14), amee.Id, pop.Id, false),
                    new Song("Ex's Hate Me", "/songs/ex-hate-me.mp3", "/images/ex-hate-me.jpg", TimeSpan.FromMinutes(4.27), amee.Id, rap.Id, false),

                    // JustaTee
                    new Song("Thằng Điên", "/songs/thang-dien.mp3", "/images/thang-dien.jpg", TimeSpan.FromMinutes(4.46), justatee.Id, pop.Id, true),

                    // Binz
                    new Song("Bigcityboi", "/songs/big-city-boi.mp3", "/images/big-city-boi.jpg", TimeSpan.FromMinutes(3.43), binz.Id, rap.Id, true)
                };

                // Tăng lượt nghe giả định
                songs[0].IncrementListenCount(); songs[0].IncrementListenCount();
                songs[4].IncrementListenCount();

                _context.Songs.AddRange(songs);
                await _context.SaveChangesAsync(); 
            }

            // ============================================================
            // 4. SEED PACKAGES (GÓI CƯỚC)
            // ============================================================
            if (!_context.SubscriptionPackages.Any())
            {
                _context.SubscriptionPackages.AddRange(
                    new SubscriptionPackage("Gói 1 Tháng (Cơ bản)", 59000, 1),
                    new SubscriptionPackage("Gói 3 Tháng (Tiết kiệm)", 159000, 3),
                    new SubscriptionPackage("Gói 6 Tháng (Phổ biến)", 299000, 6),
                    new SubscriptionPackage("Gói 1 Năm (Siêu tiết kiệm)", 599000, 12)
                );
                await _context.SaveChangesAsync(); 
            }

            // ============================================================
            // 5. SEED ORDERS & INTERACTION (FIX LỖI CHÍNH TẠI ĐÂY)
            // ============================================================


            var users = await _userManager.Users.ToListAsync();
            var dbSongs = await _context.Songs.ToListAsync();
            var packages = await _context.SubscriptionPackages.ToListAsync();

            // 5.1 Seed Orders
            if (!_context.Orders.Any() && users.Any() && packages.Any())
            {
                _context.Orders.Add(new Order(users[0].Id, packages[0].Id, packages[0].Price, 20260101000001));
                _context.Orders.Add(new Order(users[2].Id, packages[3].Id, packages[3].Price, 20260101000002));
               
            }

            // 5.2 Seed Playlist 
            if (!_context.Playlists.Any() && users.Any() && dbSongs.Count >= 3)
            {
                var playlist1 = new Playlist("Nhạc Chill Buổi Sáng", true);
                _context.Playlists.Add(playlist1);                
                await _context.SaveChangesAsync();
                if (playlist1.Id > 0)
                {
                    _context.PlaylistSongs.AddRange(
                        new PlaylistSong(playlist1.Id, dbSongs[0].Id),
                        new PlaylistSong(playlist1.Id, dbSongs[1].Id),
                        new PlaylistSong(playlist1.Id, dbSongs[2].Id)
                    );
                }

                var playlist2 = new Playlist("Nhạc Chill Buổi Tối", true);
                _context.Playlists.Add(playlist2);
                await _context.SaveChangesAsync();
                if (playlist2.Id > 0)
                {
                    _context.PlaylistSongs.AddRange(
                        new PlaylistSong(playlist2.Id, dbSongs[3].Id),
                        new PlaylistSong(playlist2.Id, dbSongs[4].Id),
                        new PlaylistSong(playlist2.Id, dbSongs[5].Id)
                    );
                }

                var playlist3 = new Playlist("Nhạc Chill Du Dương", true);
                _context.Playlists.Add(playlist3);
                await _context.SaveChangesAsync();
                if (playlist3.Id > 0)
                {
                    _context.PlaylistSongs.AddRange(
                        new PlaylistSong(playlist3.Id, dbSongs[6].Id),
                        new PlaylistSong(playlist3.Id, dbSongs[7].Id),
                        new PlaylistSong(playlist3.Id, dbSongs[8].Id)
                    );
                }

                _context.FavoriteSongs.AddRange(
                    new FavoriteSong(users[1].Id, dbSongs[0].Id),
                    new FavoriteSong(users[1].Id, dbSongs[4].Id)
                );

                await _context.SaveChangesAsync();
            }

            // ============================================================
            // 6. SEED LOG
            // ============================================================
            if (!_context.SystemLogs.Any())
            {
                var adminUser = users.FirstOrDefault(u => u.Email == adminEmail);
                if (adminUser != null)
                {
                    _context.SystemLogs.Add(new SystemLog(adminUser.Id, "Seed Data", "System", "Khởi tạo dữ liệu mẫu thành công"));
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}