using MediatR;
using MusicStreaming.Application.Common.Dtos.Admin;
using MusicStreaming.Application.Common.Interfaces.Queries;
using MusicStreaming.Application.Common.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Singers.Queries.GetSingers
{
    public class GetSingersQueryHandler : IRequestHandler<GetSingersQuery, List<SingerDto>>
    {
        private readonly ISingerQueries _singersQueries;

        public GetSingersQueryHandler(ISingerQueries singersQueries)
        {
            _singersQueries = singersQueries;
        }

        public async Task<List<SingerDto>> Handle(GetSingersQuery request, CancellationToken cancellationToken)
        {
            return  await _singersQueries.GetAllSingersAsync(cancellationToken);

        }
    }
}
