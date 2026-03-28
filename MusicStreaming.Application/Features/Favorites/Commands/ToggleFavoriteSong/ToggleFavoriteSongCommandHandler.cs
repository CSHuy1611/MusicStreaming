using MediatR;
using MusicStreaming.Application.Common.Interfaces.Repositories;
using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Favorites.Commands.ToggleFavoriteSong
{
    public class ToggleFavoriteSongCommandHandler : IRequestHandler<ToggleFavoriteSongCommand, bool>
    {
        private readonly IFavoriteSongRepository _favoriteSongRepository;
        private readonly IDashboardNotifier _dashboardNotifier;
        private readonly IUnitOfWork _unitOfWork;
        public ToggleFavoriteSongCommandHandler(IFavoriteSongRepository favoriteSongRepository, IUnitOfWork unitOfWork, IDashboardNotifier dashboardNotifier)
        {
            _favoriteSongRepository = favoriteSongRepository;
            _unitOfWork = unitOfWork;
            _dashboardNotifier = dashboardNotifier;
        }

        public async Task<bool> Handle(ToggleFavoriteSongCommand request, CancellationToken cancellationToken)
        {
            var existingFavorite = await _favoriteSongRepository.GetByUserAndSongAsync(request.UserId, request.SongId, cancellationToken);

            if (existingFavorite != null)
            {

                _favoriteSongRepository.Delete(existingFavorite);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _dashboardNotifier.BroadcastDashboardUpdateAsync();

                return false; 
            }
            else
            {
                var newFavorite = new FavoriteSong(request.UserId, request.SongId);

                await _favoriteSongRepository.AddAsync(newFavorite);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _dashboardNotifier.BroadcastDashboardUpdateAsync();

                return true; 
            }
        }
    }
}
