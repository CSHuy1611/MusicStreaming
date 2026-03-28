using MediatR;
using MusicStreaming.Application.Common.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Auth.Commands.Register
{
    public class RegisterCommand : IRequest<RegisterCommandResult>
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
