using MediatR;
using MusicStreaming.Application.Common.Dtos;
using MusicStreaming.Application.Common.Dtos.Admin;
using MusicStreaming.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Singers.Queries.GetSingers
{
    public class GetSingersQuery : IRequest<List<SingerDto>>
    {
        
    }
}
