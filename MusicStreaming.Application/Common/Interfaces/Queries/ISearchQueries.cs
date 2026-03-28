using MusicStreaming.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Interfaces.Queries
{
    public interface ISearchQueries
    {
        Task<SearchVm> SearchSongsAsync(string keyword, CancellationToken cancellationToken);
    }
}
