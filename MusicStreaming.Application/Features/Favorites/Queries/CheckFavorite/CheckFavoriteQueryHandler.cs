using MediatR;
using MusicStreaming.Application.Common.Interfaces.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Favorites.Queries.CheckFavorite
{
    public class CheckFavoriteQueryHandler : IRequestHandler<CheckFavoriteQuery, bool>
    {
        private readonly IFavoriteQueries _favoriteQueries;

        public CheckFavoriteQueryHandler(IFavoriteQueries favoriteQueries)
        {
            _favoriteQueries = favoriteQueries;
        }

        public async Task<bool> Handle(CheckFavoriteQuery request, CancellationToken cancellationToken)
        {
            return await _favoriteQueries.CheckIsFavoriteAsync(request.UserId, request.SongId, cancellationToken);
        }
    }
}
