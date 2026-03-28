using MusicStreaming.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Interfaces.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Order?> GetByOrderCodeAsync(long orderCode, CancellationToken cancellationToken = default);
    }
}
