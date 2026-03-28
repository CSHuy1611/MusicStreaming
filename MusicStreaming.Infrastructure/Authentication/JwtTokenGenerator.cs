using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MusicStreaming.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Infrastructure.Authentication
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(string userId, string userName, string email, IList<string> roles)
        {
            // 1. Tạo các Claims (Thông tin đính kèm trong token)
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId), // ID người dùng
                new Claim(JwtRegisteredClaimNames.Email, email), // Email
                new Claim(JwtRegisteredClaimNames.GivenName, userName), // Tên
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // ID riêng của Token
            };

            // Add Roles vào Claims (để phân quyền Admin/Customer)
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 2. Lấy Secret Key từ cấu hình và mã hóa
            var secretKey = _configuration["JwtSettings:Secret"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3. Thiết lập thời gian hết hạn
            var expiryMinutes = int.Parse(_configuration["JwtSettings:ExpiryMinutes"]);

            // 4. Tạo Token Object
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            // 5. Trả về chuỗi Token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
