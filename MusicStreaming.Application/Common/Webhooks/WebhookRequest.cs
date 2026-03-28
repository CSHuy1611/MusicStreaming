using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Webhooks
{
    public class WebhookRequest
    {
        public string code { get; set; } = string.Empty;
        public string desc { get; set; } = string.Empty;
        public WebhookData? data { get; set; }
        public string signature { get; set; } = string.Empty;
    }
}
