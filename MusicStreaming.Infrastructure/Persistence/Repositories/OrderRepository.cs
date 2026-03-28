using MusicStreaming.Application.Common.Interfaces.Repositories;
using MusicStreaming.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace MusicStreaming.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Order?> GetByOrderCodeAsync(long orderCode, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Order>().FirstOrDefaultAsync(o => o.OrderCode == orderCode, cancellationToken);
        }
    }
}
