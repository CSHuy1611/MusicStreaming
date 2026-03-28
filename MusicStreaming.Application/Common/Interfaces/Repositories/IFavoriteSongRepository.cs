using MusicStreaming.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Interfaces.Repositories
{
    public interface IFavoriteSongRepository : IGenericRepository<FavoriteSong>
    {
        Task<FavoriteSong> GetByUserAndSongAsync(string userId, int songId, CancellationToken cancellationToken);
    }
}
