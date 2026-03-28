using MediatR;
using MusicStreaming.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Favorites.Queries.GetFavoriteSongs
{
    public class GetFavoriteSongsQuery : IRequest<FavoriteSongsVm>
    {
        public string UserId { get; set; }
    }
}
