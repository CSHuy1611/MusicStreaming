using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MusicStreaming.Application.Common.Interfaces;
using MusicStreaming.Application.Common.Interfaces.Queries;
using MusicStreaming.Application.Common.Interfaces.Repositories;
using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.Application.Services;
using MusicStreaming.Infrastructure.Authentication;
using MusicStreaming.Infrastructure.Identity;
using MusicStreaming.Infrastructure.Persistence;
using MusicStreaming.Infrastructure.Persistence.Queries;
using MusicStreaming.Infrastructure.Persistence.Repositories;
using MusicStreaming.Infrastructure.Services;
using System;
using System.Text;

namespace MusicStreaming.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Đăng ký DbContext với SQL Server
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));


            // 2. Đăng ký Identity (User, Role) nghĩa là hệ thống sẽ sử dụng Identity để quản lý người dùng và vai trò của họ.
            // Cụ thể là ApplicationUser và ApplicationRole sẽ được sử dụng để lưu trữ thông tin người dùng và vai trò trong cơ sở dữ liệu.
            // Ngoài ra, các tùy chọn cấu hình như Password, Lockout, User cũng được thiết lập để đảm bảo an toàn và phù hợp với yêu cầu của ứng dụng.
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // Cấu hình Password
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Cấu hình Lockout nghia là nếu người dùng nhập sai mật khẩu quá nhiều lần,
                // tài khoản của họ sẽ bị khóa trong một khoảng thời gian nhất định để ngăn chặn các cuộc tấn công brute-force.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình User
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.AddScoped<ApplicationDbContextInitialiser>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            // 3. Cấu hình Cookie của Identity nghĩa là khi người dùng đăng nhập thành công,
            // hệ thống sẽ tạo một cookie để lưu thông tin phiên làm việc của họ.
            // Cookie này sẽ được gửi kèm trong mỗi yêu cầu tiếp theo để xác thực người dùng.
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Auth/Login";                  
                options.LogoutPath = "/Auth/Logout";                
                options.ExpireTimeSpan = TimeSpan.FromMinutes(120); 
                options.SlidingExpiration = true;                   
            });

            // 4. Cấu hình JWT nghĩa là khi người dùng đăng nhập thành công,
            // hệ thống sẽ tạo một token JWT chứa thông tin xác thực của họ.
            var secretKey = configuration["JwtSettings:Secret"]!;
            var key = Encoding.ASCII.GetBytes(secretKey);

            services.AddAuthentication() 
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["JwtSettings:Audience"],
                    ClockSkew = TimeSpan.Zero
                };
            })
            .AddGoogle(options =>
            {
                options.ClientId = configuration["Google:ClientId"]!;
                options.ClientSecret = configuration["Google:ClientSecret"]!;
                options.SaveTokens = true;
            });

            services.AddScoped<IIdentityService, IdentityService>();

            // Đăng ký Generic Repository và UnitOfWork
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Đăng ký các Repositories 
            services.AddScoped<IPlaylistRepository, PlaylistRepository>();
            services.AddScoped<IPlaylistSongRepository, PlaylistSongRepository>();
            services.AddScoped<IFavoriteSongRepository, FavoriteSongRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ISongRepository, SongRepository>();

            // Đăng ký các Queries
            services.AddScoped<IHomeQueries, HomeQueries>();
            services.AddScoped<ILibraryQueries, LibraryQueries>();
            services.AddScoped<IPlaylistQueries, PlaylistQueries>();
            services.AddScoped<ISearchQueries, SearchQueries>();
            services.AddScoped<IFavoriteQueries, FavoriteQueries>();
            services.AddScoped<ISubscriptionQueries, SubscriptionQueries>();
            services.AddScoped<ISingerQueries, SingerQueries>();
            services.AddScoped<IGenreQueries, GenreQueries>();
            services.AddScoped<ISongQueries, SongQueries>();
            services.AddScoped<IDashboardQueries, DashboardQueries>();

            // Đăng ký các Services
            services.AddScoped<ISongAdminService, SongAdminService>();
            services.AddScoped<ISingerAdminService, SingerAdminService>();
            services.AddScoped<ILibraryAdminService, LibraryAdminService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<ISubscriptionAdminService, SubscriptionAdminService>();
            services.AddScoped<IPaymentService, PayOSService>();


            return services;
        }
    }
}