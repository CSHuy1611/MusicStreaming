using MediatR;
using MusicStreaming.Application.Common.Dtos;
using MusicStreaming.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Subscriptions.Queries.GetActivePackages
{
    public class GetActivePackagesQuery : IRequest<PackageVm>
    {
    }
}
