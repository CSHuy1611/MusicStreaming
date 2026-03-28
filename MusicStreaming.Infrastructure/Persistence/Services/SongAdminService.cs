using MusicStreaming.Application.Common.Dtos.Admin;
using MusicStreaming.Application.Common.Exceptions;
using MusicStreaming.Application.Common.Interfaces.Repositories;
using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Services
{
    public class SongAdminService : ISongAdminService
    {
        private readonly IGenericRepository<Singer> _singerRepository;
        private readonly IGenericRepository<Genre> _genreRepository;
        private readonly ISongRepository _songRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDashboardNotifier _dashboardNotifier;

        public SongAdminService(
            IGenericRepository<Singer> singerRepository,
            IGenericRepository<Genre> genreRepository,
            ISongRepository songRepository,
            IUnitOfWork unitOfWork,
            IDashboardNotifier dashboardNotifier) 
        {
            _songRepository = songRepository;
            _singerRepository = singerRepository;
            _genreRepository = genreRepository;
            _unitOfWork = unitOfWork;
            _dashboardNotifier = dashboardNotifier;
        }

        public async Task<IEnumerable<SongAdminDto>> GetAllSongsAsync(CancellationToken cancellationToken = default)
        {
            var songs = await _songRepository.GetAllWithDetailsAsync(cancellationToken);

            var dtos = songs.Where(s => !s.IsDeleted).Select(s => new SongAdminDto
            {
                Id = s.Id,
                Title = s.Title,
                Duration = s.Duration,
                IsVip = s.IsVip,
                ListenCount = s.ListenCount,
                AudioUrl = s.AudioUrl,
                ImageUrl = s.ImageUrl,
                SingerId = s.SingerId,
                GenreId = s.GenreId,
                SingerName = s.Singer?.Name ?? "Unknown",
                GenreName = s.Genre?.Name ?? "Unknown"
            });

            return dtos.ToList();
        }

        public async Task<SongAdminDto> CreateSongAsync(CreateSongDto dto, CancellationToken cancellationToken = default)
        {
            bool isDuplicate = await _songRepository.ExistsAsync(s => s.Title == dto.Title && !s.IsDeleted);

            if (isDuplicate)
            {
                throw new BadRequestException($"Bài hát với tên '{dto.Title}' đã tồn tại trong hệ thống.");
            }


            string audioUrl = !string.IsNullOrEmpty(dto.AudioUrl) ? dto.AudioUrl : string.Empty;
            string imageUrl = !string.IsNullOrEmpty(dto.ImageUrl) ? dto.ImageUrl : string.Empty;

 
            var song = new Song(
                title: dto.Title,
                audioUrl: audioUrl,
                imageUrl: imageUrl,
                duration: dto.Duration,
                singerId: dto.SingerId,
                genreId: dto.GenreId,
                isVip: dto.IsVip
            );

            await _songRepository.AddAsync(song);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _dashboardNotifier.BroadcastDashboardUpdateAsync();

            return await MapToDtoAsync(song);
        }

        public async Task<SongAdminDto> UpdateSongAsync(UpdateSongDto dto, CancellationToken cancellationToken = default)
        {
            var song = await _songRepository.GetByIdAsync(dto.Id);
            if (song == null || song.IsDeleted) throw new Exception($"Song with Id {dto.Id} not found");

            bool isDuplicate = await _songRepository.ExistsAsync(s => s.Id != dto.Id && s.Title == dto.Title && !s.IsDeleted);

            if (isDuplicate)
            {
                throw new BadRequestException($"Bài hát với tên '{dto.Title}' đã tồn tại trong hệ thống.");
            }


            string newAudioUrl = !string.IsNullOrEmpty(dto.AudioUrl) ? dto.AudioUrl : song.AudioUrl;
            string newImageUrl = !string.IsNullOrEmpty(dto.ImageUrl) ? dto.ImageUrl : song.ImageUrl;

            song.UpdateFullInfo(dto.Title, newAudioUrl, newImageUrl, dto.Duration, dto.SingerId, dto.GenreId, dto.IsVip);

            _songRepository.Update(song);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _dashboardNotifier.BroadcastDashboardUpdateAsync();

            return await MapToDtoAsync(song);
        }

        public async Task<bool> DeleteSongAsync(int id, CancellationToken cancellationToken = default)
        {
            var song = await _songRepository.GetByIdAsync(id);
            if (song == null) return false;

            song.Delete();

            _songRepository.Update(song);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _dashboardNotifier.BroadcastDashboardUpdateAsync();

            return true;
        }

        private async Task<SongAdminDto> MapToDtoAsync(Song song)
        {
            var singer = await _singerRepository.GetByIdAsync(song.SingerId);
            var genre = await _genreRepository.GetByIdAsync(song.GenreId);

            return new SongAdminDto
            {
                Id = song.Id,
                Title = song.Title,
                Duration = song.Duration,
                IsVip = song.IsVip,
                ListenCount = song.ListenCount,
                AudioUrl = song.AudioUrl,
                ImageUrl = song.ImageUrl,
                SingerId = song.SingerId,
                GenreId = song.GenreId,
                SingerName = singer?.Name ?? "Unknown",
                GenreName = genre?.Name ?? "Unknown"
            };
        }

        
    }
}