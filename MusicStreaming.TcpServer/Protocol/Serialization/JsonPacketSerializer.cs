using System.Text;
using System.Text.Json;

namespace MusicStreaming.TcpServer.Protocol.Serialization
{
    public class JsonPacketSerializer : IPacketSerializer
    {
        private readonly JsonSerializerOptions _options;

        public JsonPacketSerializer()
        {
            _options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }

        public byte[] Serialize<T>(T obj)
        {
            var jsonString = JsonSerializer.Serialize(obj, _options);
            return Encoding.UTF8.GetBytes(jsonString);
        }

        public T? Deserialize<T>(byte[] data)
        {
            var jsonString = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<T>(jsonString, _options);
        }
    }
}
