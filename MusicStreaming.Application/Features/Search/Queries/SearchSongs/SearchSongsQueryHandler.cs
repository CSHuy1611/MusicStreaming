using MediatR;
using MusicStreaming.Application.Common.Interfaces.Queries;
using MusicStreaming.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Search.Queries.SearchSongs
{
    public class SearchSongsQueryHandler : IRequestHandler<SearchSongsQuery, SearchVm>
    {
        private readonly ISearchQueries _searchQueries;

        public SearchSongsQueryHandler(ISearchQueries searchQueries)
        {
            _searchQueries = searchQueries;
        }

        public async Task<SearchVm> Handle(SearchSongsQuery request, CancellationToken cancellationToken)
        {
 
            if (string.IsNullOrWhiteSpace(request.Keyword))
            {
                return new SearchVm
                {
                    Keyword = request.Keyword,
                    Songs = new List<Common.Dtos.SongDto>()
                };
            }

            return await _searchQueries.SearchSongsAsync(request.Keyword, cancellationToken);
        }
    }
}
