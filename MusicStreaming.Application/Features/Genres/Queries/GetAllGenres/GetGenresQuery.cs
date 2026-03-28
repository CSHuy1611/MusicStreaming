using MediatR;
using MusicStreaming.Application.Common.Dtos.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Genres.Queries.GetAllGenres
{
    public class GetGenresQuery : IRequest<List<GenreDto>>
    {
    }
}
