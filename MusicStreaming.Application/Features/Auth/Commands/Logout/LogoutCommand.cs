using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommand : IRequest<bool> { }
}
