using System;

namespace MusicStreaming.TcpServer.Protocol.Models
{
    public class TcpRequest
    {
        public Guid RequestId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
    }
}
