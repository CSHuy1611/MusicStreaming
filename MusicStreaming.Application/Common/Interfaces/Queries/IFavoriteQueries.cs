using MusicStreaming.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Interfaces.Queries
{
    public interface IFavoriteQueries
    {
        Task<FavoriteSongsVm> GetUserFavoriteSongsAsync(string userId, CancellationToken cancellationToken);
        Task<bool> CheckIsFavoriteAsync(string userId, int songId, CancellationToken cancellationToken);
    }
}
