using MediatR;
using MusicStreaming.Application.Common.Interfaces.Queries;
using MusicStreaming.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Favorites.Queries.GetFavoriteSongs
{
    public class GetFavoriteSongsQueryHandler : IRequestHandler<GetFavoriteSongsQuery, FavoriteSongsVm>
    {
        private readonly IFavoriteQueries _favoriteQueries;

        public GetFavoriteSongsQueryHandler(IFavoriteQueries favoriteQueries)
        {
            _favoriteQueries = favoriteQueries;
        }

        public async Task<FavoriteSongsVm> Handle(GetFavoriteSongsQuery request, CancellationToken cancellationToken)
        {
            return await _favoriteQueries.GetUserFavoriteSongsAsync(request.UserId, cancellationToken);
        }
    }
}
