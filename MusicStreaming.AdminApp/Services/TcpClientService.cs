using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MusicStreaming.AdminApp.Models;

namespace MusicStreaming.AdminApp.Services
{
    public class TcpClientService
    {
        private readonly string _ip;
        private readonly int _port;

        public TcpClientService(string ip = "127.0.0.1", int port = 5000)
        {
            _ip = ip;
            _port = port;
        }

        public async Task<TcpResponse> SendRequestAsync(string action, object payload)
        {
            try
            {
                using var client = new TcpClient();

                client.SendBufferSize = 25 * 1024 * 1024;
                client.ReceiveBufferSize = 25 * 1024 * 1024;
                client.SendTimeout = 60000;
                client.ReceiveTimeout = 60000;

                await client.ConnectAsync(_ip, _port);
                using var stream = client.GetStream();

                var request = new TcpRequest
                {
                    Action = action,
                    Payload = JsonSerializer.Serialize(payload)
                };

                var jsonRequest = JsonSerializer.Serialize(request);
                var requestBytes = Encoding.UTF8.GetBytes(jsonRequest);
                var lengthBytes = BitConverter.GetBytes(requestBytes.Length);

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(lengthBytes);
                }

                await stream.WriteAsync(lengthBytes, 0, lengthBytes.Length);
                await stream.WriteAsync(requestBytes, 0, requestBytes.Length);


                var responseLengthBytes = new byte[4];
                int bytesRead = await stream.ReadAsync(responseLengthBytes, 0, 4);
                if (bytesRead < 4) throw new Exception("Failed to read response length.");

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(responseLengthBytes);
                }

                int messageLength = BitConverter.ToInt32(responseLengthBytes, 0);
                var responseBuffer = new byte[messageLength];
                int totalRead = 0;
                while (totalRead < messageLength)
                {
                    int read = await stream.ReadAsync(responseBuffer, totalRead, messageLength - totalRead);
                    if (read == 0) throw new Exception("Connection closed prematurely.");
                    totalRead += read;
                }

                var jsonResponse = Encoding.UTF8.GetString(responseBuffer);
                try
                {
                    var responseObj = JsonSerializer.Deserialize<TcpResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (responseObj != null && !responseObj.IsSuccess && string.IsNullOrEmpty(responseObj.Data) && string.IsNullOrEmpty(responseObj.Message))
                    {
                        responseObj.Data = $"Raw Server Response: {jsonResponse}";
                    }

                    return responseObj ?? new TcpResponse { StatusCode = 500, Data = "Deserialization failed." };
                }
                catch (JsonException jsonEx)
                {
                    return new TcpResponse { StatusCode = 500, Data = $"JSON Error: {jsonEx.Message}. Raw: {jsonResponse}" };
                }
            }
            catch (Exception ex)
            {
                return new TcpResponse { StatusCode = 500, Data = $"Transport Error: {ex.Message}" };
            }
        }
    }
}