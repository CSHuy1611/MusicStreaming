using MediatR;
using MusicStreaming.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Search.Queries.SearchSongs
{
    public class SearchSongsQuery : IRequest<SearchVm>
    {
        public string Keyword { get; set; }
    }
}
