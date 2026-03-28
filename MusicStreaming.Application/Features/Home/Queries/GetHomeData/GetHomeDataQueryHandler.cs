using MediatR;
using MusicStreaming.Application.Common.Dtos;
using MusicStreaming.Application.Common.Interfaces;
using MusicStreaming.Application.Common.Interfaces.Queries;
using MusicStreaming.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Home.Queries.GetHomeData
{
    public class GetHomeDataQueryHandler : IRequestHandler<GetHomeDataQuery, HomeDataVm>
    {
        private readonly IHomeQueries _homeQueries;
        private readonly IIdentityService _identityService;

        public GetHomeDataQueryHandler(IHomeQueries homeQueries, IIdentityService identityService)
        {
            _homeQueries = homeQueries;
            _identityService = identityService;
        }

        public async Task<HomeDataVm> Handle(GetHomeDataQuery request, CancellationToken cancellationToken)
        {
            // Ủy quyền toàn bộ logic query phức tạp cho IHomeQueries (Tầng Infrastructure sẽ lo việc dùng EF Core)
            var vm = await _homeQueries.GetHomeDataAsync(cancellationToken);

            // Gắn thêm logic nghiệp vụ của Identity
            vm.IsUserVip = await _identityService.IsUserVipAsync(request.UserId);

            return vm;
        }
    }
}

