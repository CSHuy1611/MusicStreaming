using MusicStreaming.Application.Common.Dtos.Admin;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Interfaces.Services
{
    public interface ISongAdminService
    {
        Task<IEnumerable<SongAdminDto>> GetAllSongsAsync(CancellationToken cancellationToken = default);
        Task<SongAdminDto> CreateSongAsync(CreateSongDto dto, CancellationToken cancellationToken = default);
        Task<SongAdminDto> UpdateSongAsync(UpdateSongDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteSongAsync(int id, CancellationToken cancellationToken = default);
    }
}
