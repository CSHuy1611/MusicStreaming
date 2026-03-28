using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Subscriptions.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<string>
    {
        public string UserId { get; set; } = string.Empty;
        public int PackageId { get; set; }
    }
}
