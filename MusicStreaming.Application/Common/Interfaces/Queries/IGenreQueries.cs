using MusicStreaming.Application.Common.Dtos.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Interfaces.Queries
{
    public interface IGenreQueries
    {
        Task<List<GenreDto>> GetAllGenresAsync(CancellationToken cancellationToken);
    }
}
