using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Models.Results
{
    public class AuthenticationResult
    {
        public bool IsSuccess { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public IEnumerable<string> Errors { get; set; }

        // Hàm tiện ích để trả về kết quả thành công
        public static AuthenticationResult Success(string token, string userId, string fullName)
        {
            return new AuthenticationResult { IsSuccess = true, Token = token, UserId = userId, FullName = fullName };
        }

        // Hàm tiện ích để trả về lỗi
        public static AuthenticationResult Failure(IEnumerable<string> errors)
        {
            return new AuthenticationResult { IsSuccess = false, Errors = errors };
        }
    }
}
