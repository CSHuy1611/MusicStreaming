using MediatR;
using MusicStreaming.Application.Common.Interfaces;
using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.Application.Common.Models.Results;
using System.Threading;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterCommandResult>
    {
        private readonly IIdentityService _identityService;
        private readonly IDashboardNotifier _dashboardNotifier;

        public RegisterCommandHandler(
            IIdentityService identityService,
            IDashboardNotifier dashboardNotifier)
        {
            _identityService = identityService;
            _dashboardNotifier = dashboardNotifier;
        }

        public async Task<RegisterCommandResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var (isSuccess, userId, errors) = await _identityService.RegisterUserAsync(request.Email, request.Password, request.FullName);

            if (!isSuccess)
            {
                return new RegisterCommandResult
                {
                    IsSuccess = false,
                    Errors = errors
                };
            }

            await _dashboardNotifier.BroadcastDashboardUpdateAsync();

            await _identityService.SignInUserAsync(userId);

            return new RegisterCommandResult
            {
                IsSuccess = true,
                UserId = userId
            };
        }
    }
}