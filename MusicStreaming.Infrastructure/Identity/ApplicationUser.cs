using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }

        public bool IsPremium { get; private set; }
        public DateTime? PremiumExpireDate { get; private set; }

        public bool IsValidPremium()
        {
            return IsPremium && PremiumExpireDate.HasValue && PremiumExpireDate.Value > DateTime.UtcNow;
        }

        public void ExtendPremium(int months)
        {
            if (months <= 0) throw new ArgumentException("Số tháng không hợp lệ.");


            if (IsValidPremium())
            {
                PremiumExpireDate = PremiumExpireDate.Value.AddMonths(months);
            }
            else
            {

                IsPremium = true;
                PremiumExpireDate = DateTime.UtcNow.AddMonths(months);
            }
        }

        public void CancelPremium()
        {
            IsPremium = false;
            PremiumExpireDate = null;
        }
    }
}
