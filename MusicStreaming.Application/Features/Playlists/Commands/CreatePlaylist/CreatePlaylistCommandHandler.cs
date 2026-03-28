using MediatR;
using MusicStreaming.Application.Common.Interfaces.Repositories;
using MusicStreaming.Application.Features.Playlists.Commands.AddSongToPlaylist;
using MusicStreaming.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Playlists.Commands.CreatePlaylist
{
    public class CreatePlaylistCommandHandler : IRequestHandler<CreatePlaylistCommand, int>
    {
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePlaylistCommandHandler(IPlaylistRepository playlistRepository, IUnitOfWork unitOfWork)
        {
            _playlistRepository = playlistRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(CreatePlaylistCommand request, CancellationToken cancellationToken)
        {
            var entity = new Playlist(request.UserId, request.Name);

            await _playlistRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}
