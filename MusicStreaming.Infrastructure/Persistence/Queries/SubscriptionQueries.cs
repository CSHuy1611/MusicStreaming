using Microsoft.EntityFrameworkCore;
using MusicStreaming.Application.Common.Dtos;
using MusicStreaming.Application.Common.Interfaces.Queries;
using MusicStreaming.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Infrastructure.Persistence.Queries
{
    public class SubscriptionQueries : ISubscriptionQueries
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionQueries(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PackageVm> GetActivePackagesAsync(CancellationToken cancellationToken)
        {
            var packages = await _context.SubscriptionPackages
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.Price)
                .Select(p => new PackageDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    DurationInMonths = p.DurationInMonths
                })
                .ToListAsync(cancellationToken);

            return new PackageVm { Packages = packages };
        }


    }
}
