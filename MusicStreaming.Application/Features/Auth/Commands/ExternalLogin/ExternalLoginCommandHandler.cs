using MediatR;
using MusicStreaming.Application.Common.Interfaces;
using MusicStreaming.Application.Common.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Auth.Commands.ExternalLogin
{
    public class ExternalLoginCommandHandler : IRequestHandler<ExternalLoginCommand, AuthenticationResult>
    {
        private readonly IIdentityService _identityService;

        public ExternalLoginCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<AuthenticationResult> Handle(ExternalLoginCommand request, CancellationToken cancellationToken)
        {
            return await _identityService.ExternalLoginAsync(
                request.Email, 
                request.FullName, 
                request.Provider, 
                request.ProviderKey);
        }
    }
}
