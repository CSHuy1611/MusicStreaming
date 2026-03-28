using MediatR;
using MusicStreaming.Application.Common.Dtos;
using MusicStreaming.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Songs.Queries.GetAllSongs
{
    public class GetAllSongsQuery : IRequest<PagedResult<SongDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}
