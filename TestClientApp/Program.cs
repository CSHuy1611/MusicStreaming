using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Connecting to TCP Server...");
        using var client = new TcpClient("127.0.0.1", 5000);
        using var stream = client.GetStream();
        Console.WriteLine("Connected!");

        var request = new
        {
            RequestId = Guid.NewGuid(),
            Action = "TestConnection",
            Payload = "{ \"Message\": \"Hello from Admin\" }"
        };

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var requestJson = JsonSerializer.Serialize(request, jsonOptions);
        var requestBytes = Encoding.UTF8.GetBytes(requestJson);

        var lengthPrefix = BitConverter.GetBytes(requestBytes.Length);
        await stream.WriteAsync(lengthPrefix, 0, 4);
        await stream.WriteAsync(requestBytes, 0, requestBytes.Length);

        Console.WriteLine("Sent data. Waiting for response...");

        var rspLengthBuffer = new byte[4];
        await stream.ReadAsync(rspLengthBuffer, 0, 4);
        int rspLength = BitConverter.ToInt32(rspLengthBuffer, 0);

        var rspDataBuffer = new byte[rspLength];
        await stream.ReadAsync(rspDataBuffer, 0, rspLength);
        
        var rspJson = Encoding.UTF8.GetString(rspDataBuffer);
        Console.WriteLine($"Response Received:\n{rspJson}");
    }
}
