using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.TcpServer.Routing.Interfaces;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MusicStreaming.TcpServer.Routing.Endpoints.Songs
{
    public class DeleteSongEndpoint : IEndpointHandler
    {
        private readonly ISongAdminService _songAdminService;

        public DeleteSongEndpoint(ISongAdminService songAdminService)
        {
            _songAdminService = songAdminService;
        }

        public string ActionName => "DeleteSong";

        public async Task<object> HandleAsync(string payload, CancellationToken cancellationToken)
        {
            // Expecting payload to be the string "1" (the integer ID of the song) or an object wrapper.
            // Easiest is to send an anonymous object or just deserializing string as int.
            int id;
            if (int.TryParse(payload, out int simpleId))
            {
                id = simpleId;
            }
            else
            {
                try
                {
                    var doc = JsonDocument.Parse(payload);
                    id = doc.RootElement.GetProperty("Id").GetInt32();
                }
                catch
                {
                    throw new ArgumentException("Payload is invalid. Expected an integer or { \"Id\": <int> }.");
                }
            }

            var success = await _songAdminService.DeleteSongAsync(id, cancellationToken);
            if (!success)
            {
                throw new Exception($"Song with Id {id} not found or could not be deleted.");
            }

            return new { Success = true, DeletedId = id };
        }
    }
}
