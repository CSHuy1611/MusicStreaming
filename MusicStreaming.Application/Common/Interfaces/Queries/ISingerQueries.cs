using MusicStreaming.Application.Common.Dtos.Admin;
using MusicStreaming.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Interfaces.Queries
{
    public interface ISingerQueries
    {
        Task<ArtistProfileVm> GetArtistProfileAsync(int singerId, CancellationToken cancellationToken);
        Task<List<SingerDto>> GetAllSingersAsync(CancellationToken cancellationToken);
    }
}
