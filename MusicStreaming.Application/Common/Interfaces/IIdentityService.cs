using MusicStreaming.Application.Common.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> LoginAsync(string email, string password);
        Task<(bool IsSuccess, string UserId, List<string> Errors)> RegisterUserAsync(string email, string password, string fullName);
        Task SignInUserAsync(string userId);
        Task SignOutUserAsync();
        Task<bool> IsUserVipAsync(string userId);
        Task<bool> UpgradeVipAsync(string userId, int monthsToExtend);
        Task<AuthenticationResult> ExternalLoginAsync(string email, string fullName, string provider, string providerKey);
        Task<int> GetTotalUsersCountAsync();
        Task<int> GetTotalVipUsersCountAsync();
    }
}
