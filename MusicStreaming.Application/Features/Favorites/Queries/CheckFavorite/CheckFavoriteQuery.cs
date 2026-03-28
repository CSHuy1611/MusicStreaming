using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Favorites.Queries.CheckFavorite
{
    public class CheckFavoriteQuery : IRequest<bool>
    {
        public string UserId { get; set; }
        public int SongId { get; set; }
    }
}
