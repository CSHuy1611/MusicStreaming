using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MusicStreaming.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenGenerator _tokenGenerator;

        public IdentityService(UserManager<ApplicationUser> userManager,
                                IJwtTokenGenerator tokenGenerator,
                                RoleManager<ApplicationRole> roleManager,
                                SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _tokenGenerator = tokenGenerator;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<Application.Common.Models.Results.AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Application.Common.Models.Results.AuthenticationResult.Failure(new[] { "Tài khoản không tồn tại." });
            }
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid)
            {
                return Application.Common.Models.Results.AuthenticationResult.Failure(new[] { "Mật khẩu không chính xác." });
            }
            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenGenerator.GenerateToken(user.Id, user.FullName, user.Email, roles);
            return Application.Common.Models.Results.AuthenticationResult.Success(token, user.Id, user.FullName);
        }

        public async Task<bool> IsUserVipAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && user.IsPremium && user.PremiumExpireDate > DateTime.UtcNow)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UpgradeVipAsync(string userId, int monthsToExtend)
        {
            if (string.IsNullOrEmpty(userId) || monthsToExtend <= 0) return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            try
            {
                user.ExtendPremium(monthsToExtend);
                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _userManager.Users.CountAsync();
        }

        public async Task<int> GetTotalVipUsersCountAsync()
        {
            var now = DateTime.UtcNow;
            return await _userManager.Users.CountAsync(u => u.IsPremium && u.PremiumExpireDate > now);
        }

        public async Task<(bool IsSuccess, string UserId, List<string> Errors)> RegisterUserAsync(string email, string password, string fullName)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return (false, string.Empty, result.Errors.Select(e => e.Description).ToList());
            }


            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new ApplicationRole("User")
                {
                    Description = "Người dùng cơ bản của hệ thống"
                });
            }


            await _userManager.AddToRoleAsync(user, "User");

            return (true, user.Id, new List<string>());
        }

        public async Task<Application.Common.Models.Results.AuthenticationResult> ExternalLoginAsync(string email, string fullName, string provider, string providerKey)
        {
            var user = await _userManager.FindByLoginAsync(provider, providerKey);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FullName = fullName
                    };
                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        return Application.Common.Models.Results.AuthenticationResult.Failure(result.Errors.Select(e => e.Description));
                    }

                    if (!await _roleManager.RoleExistsAsync("User"))
                    {
                        await _roleManager.CreateAsync(new ApplicationRole("User") { Description = "Người dùng cơ bản" });
                    }
                    await _userManager.AddToRoleAsync(user, "User");
                }

                var addLoginResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerKey, provider));
                if (!addLoginResult.Succeeded)
                {
                    return Application.Common.Models.Results.AuthenticationResult.Failure(addLoginResult.Errors.Select(e => e.Description));
                }
            }

            await _signInManager.SignInAsync(user, isPersistent: true);
            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenGenerator.GenerateToken(user.Id, user.FullName, user.Email, roles);

            return Application.Common.Models.Results.AuthenticationResult.Success(token, user.Id, user.FullName);
        }

        public async Task SignInUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _signInManager.SignInAsync(user, isPersistent: true);
            }
        }

        public async Task SignOutUserAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
