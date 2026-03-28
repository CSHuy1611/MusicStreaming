using MediatR;
using MusicStreaming.Application.Common.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Auth.Commands.Login
{
    // IRequest<AuthenticationResult>: Yêu cầu này sẽ trả về kết quả là AuthenticationResult
    public class LoginCommand : IRequest<AuthenticationResult>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
