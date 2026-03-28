using MediatR;
using MusicStreaming.Application.Common.Interfaces.Repositories;
using MusicStreaming.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Playlists.Commands.AddSongToPlaylist
{
    public class AddSongToPlaylistCommandHandler : IRequestHandler<AddSongToPlaylistCommand, bool>
    {
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IPlaylistSongRepository _playlistSongRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddSongToPlaylistCommandHandler(IPlaylistRepository playlistRepository, IPlaylistSongRepository playlistSongRepository, IUnitOfWork unitOfWork)
        {
            _playlistRepository = playlistRepository;
            _playlistSongRepository = playlistSongRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(AddSongToPlaylistCommand request, CancellationToken cancellationToken)
        {
            // 1. Kiểm tra Playlist có tồn tại và thuộc về User không
            var playlist = await _playlistRepository.GetByIdAndUserIdAsync(request.PlaylistId, request.UserId, cancellationToken);

            if (playlist == null)
            {
                return false;
            }

            // 2. Kiểm tra bài hát đã có trong Playlist chưa
            bool isExist = await _playlistSongRepository.IsSongExistInPlaylistAsync(request.PlaylistId, request.SongId, cancellationToken);

            if (isExist)
            {
                return true;
            }

            // 3. Thêm mới
            var playlistSong = new PlaylistSong(request.PlaylistId, request.SongId);

            await _playlistSongRepository.AddAsync(playlistSong);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
