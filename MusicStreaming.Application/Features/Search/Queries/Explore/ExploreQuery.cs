using MediatR;
using MusicStreaming.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Search.Queries.Explore
{
    public class ExploreQuery : IRequest<ExploreVm>
    {
        public string? Keyword { get; set; }
        public int? GenreId { get; set; }
        public int? SingerId { get; set; }
        public string? SortBy { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12; 
    }
}
