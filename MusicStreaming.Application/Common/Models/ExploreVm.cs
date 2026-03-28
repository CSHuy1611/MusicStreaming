using MusicStreaming.Application.Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Models
{
    public class FilterOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ExploreVm
    {
        public bool IsSearching { get; set; }
        public List<FilterOptionDto> Genres { get; set; } = new List<FilterOptionDto>();
        public List<FilterOptionDto> Singers { get; set; } = new List<FilterOptionDto>();
        public List<SongDto> RecommendedSongs { get; set; } = new List<SongDto>();
        public PagedResult<SongDto> SearchResults { get; set; } = new PagedResult<SongDto>();
    }
}
