using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Webhooks
{
    public class WebhookData
    {
        public long orderCode { get; set; }
        public int amount { get; set; }
        public string description { get; set; } = string.Empty;
        public string reference { get; set; } = string.Empty;
        public string transactionDateTime { get; set; } = string.Empty;
        public string paymentLinkId { get; set; } = string.Empty;
    }
}
