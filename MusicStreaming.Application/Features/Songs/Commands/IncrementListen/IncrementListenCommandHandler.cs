using MediatR;
using MusicStreaming.Application.Common.Interfaces.Repositories;
using MusicStreaming.Application.Common.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Songs.Commands.IncrementListen
{
    public class IncrementListenCommandHandler : IRequestHandler<IncrementListenCommand, bool>
    {
        private readonly ISongRepository _songRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDashboardNotifier _dashboardNotifier;

        public IncrementListenCommandHandler(
            ISongRepository songRepository,
            IUnitOfWork unitOfWork,
            IDashboardNotifier dashboardNotifier)
        {
            _songRepository = songRepository;
            _unitOfWork = unitOfWork;
            _dashboardNotifier = dashboardNotifier;
        }

        public async Task<bool> Handle(IncrementListenCommand request, CancellationToken cancellationToken)
        {

            var isSuccess = await _songRepository.IncrementListenCountAsync(request.SongId, cancellationToken);
            await _dashboardNotifier.BroadcastDashboardUpdateAsync();

            return isSuccess;
        }
    }
}
