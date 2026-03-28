using MediatR;
using MusicStreaming.Application.Common.Dtos;
using MusicStreaming.Application.Common.Interfaces.Queries;
using MusicStreaming.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Subscriptions.Queries.GetActivePackages
{
    public class GetActivePackagesQueryHandler : IRequestHandler<GetActivePackagesQuery, PackageVm>
    {
        private readonly ISubscriptionQueries _subscriptionQueries;

        public GetActivePackagesQueryHandler(ISubscriptionQueries subscriptionQueries)
        {
            _subscriptionQueries = subscriptionQueries;
        }

        public async Task<PackageVm> Handle(GetActivePackagesQuery request, CancellationToken cancellationToken)
        {
            return await _subscriptionQueries.GetActivePackagesAsync(cancellationToken);
        }
    }
}
