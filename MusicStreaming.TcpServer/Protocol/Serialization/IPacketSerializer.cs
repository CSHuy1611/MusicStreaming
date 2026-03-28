using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Protocol.Serialization
{
    public interface IPacketSerializer
    {
        byte[] Serialize<T>(T obj);
        T? Deserialize<T>(byte[] data);
    }
}
