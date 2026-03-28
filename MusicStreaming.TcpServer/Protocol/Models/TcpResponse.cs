using System;

namespace MusicStreaming.TcpServer.Protocol.Models
{
    public class TcpResponse
    {
        public Guid RequestId { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;

        public static TcpResponse Success(Guid requestId, string data) => new()
        {
            RequestId = requestId,
            StatusCode = 200,
            Message = "Success",
            Data = data
        };

        public static TcpResponse Error(Guid requestId, int statusCode, string message) => new()
        {
            RequestId = requestId,
            StatusCode = statusCode,
            Message = message,
            Data = string.Empty
        };
    }
}
