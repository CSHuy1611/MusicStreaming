using MediatR;
using MusicStreaming.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Home.Queries.GetHomeData
{
    public class GetHomeDataQuery : IRequest<HomeDataVm>
    {
        public string UserId { get; set; }
    }
}
