using MusicStreaming.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Domain.Entities
{
    public class SubscriptionPackage : BaseEntity
    {
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public int DurationInMonths { get; private set; }
        public bool IsActive { get; private set; }

        public SubscriptionPackage(string name, decimal price, int durationInMonths)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Tên gói trống.");
            if (price < 0) throw new ArgumentException("Giá không được âm.");
            if (durationInMonths <= 0) throw new ArgumentException("Thời hạn phải lớn hơn 0.");

            Name = name;
            Price = price;
            DurationInMonths = durationInMonths;
            IsActive = true; 
        }

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice < 0) throw new ArgumentException("Giá không hợp lệ.");
            Price = newPrice;
            UpdateTime();
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdateTime();
        }

        public void Activate()
        {
            IsActive = true;
            UpdateTime();
        }

        public void UpdateFullInfo(string name, decimal price, int durationInMonths, bool isActive)
        {
            if (!string.IsNullOrWhiteSpace(name)) Name = name;
            UpdatePrice(price);
            if (durationInMonths > 0) DurationInMonths = durationInMonths;
            IsActive = isActive;
            UpdateTime();
        }
    }
}
