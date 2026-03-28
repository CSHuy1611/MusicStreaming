using MusicStreaming.Domain.Common;
using MusicStreaming.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Domain.Entities
{
    public class Order : BaseEntity
    {
        public string UserId { get; private set; } 
        public int PackageId { get; private set; }
        public decimal Amount { get; private set; }
        public long OrderCode { get; private set; } 
        public OrderStatus Status { get; private set; }
        public DateTime PurchaseDate { get; private set; }

        public SubscriptionPackage Package { get; private set; }

        public Order(string userId, int packageId, decimal amount, long orderCode)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User không hợp lệ.");
            if (amount < 0) throw new ArgumentException("Số tiền không hợp lệ.");

            UserId = userId;
            PackageId = packageId;
            Amount = amount;
            OrderCode = orderCode;
            Status = OrderStatus.Pending;
            PurchaseDate = DateTime.UtcNow;
        }

        public void MarkAsPaid()
        {
            Status = OrderStatus.Paid;
            UpdateTime();
        }

        public void MarkAsCancelled()
        {
            Status = OrderStatus.Cancelled;
            UpdateTime();
        }
    }
}
