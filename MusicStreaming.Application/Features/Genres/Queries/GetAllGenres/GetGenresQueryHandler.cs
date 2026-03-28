using MediatR;
using MusicStreaming.Application.Common.Dtos.Admin;
using MusicStreaming.Application.Common.Interfaces.Queries;
using MusicStreaming.Application.Common.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Genres.Queries.GetAllGenres
{
    public class GetGenresQueryHandler : IRequestHandler<GetGenresQuery, List<GenreDto>>
    {
        private readonly IGenreQueries _genreQueries;

        public GetGenresQueryHandler(IGenreQueries genreQueries)
        {
            _genreQueries = genreQueries;
        }

        public async Task<List<GenreDto>> Handle(GetGenresQuery request, CancellationToken cancellationToken)
        {
            return await _genreQueries.GetAllGenresAsync(cancellationToken);
        }
    }
}
