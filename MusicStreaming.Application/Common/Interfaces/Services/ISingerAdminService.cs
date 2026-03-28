using MusicStreaming.Application.Common.Dtos.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Interfaces.Services
{
    public interface ISingerAdminService
    {
        Task<IEnumerable<SingerAdminDto>> GetAllSingersAsync(CancellationToken cancellationToken = default);
        Task<SingerAdminDto> CreateSingerAsync(CreateSingerDto dto, CancellationToken cancellationToken = default);
        Task<SingerAdminDto> UpdateSingerAsync(UpdateSingerDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteSingerAsync(int id, CancellationToken cancellationToken = default);
    }
}
