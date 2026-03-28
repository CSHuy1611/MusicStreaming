using MusicStreaming.Application.Common.Dtos.Admin;
using System.Threading;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Interfaces.Queries
{
    public interface IDashboardQueries
    {
        Task<DashboardDataDto> GetDashboardDataAsync(CancellationToken cancellationToken = default);
    }
}
