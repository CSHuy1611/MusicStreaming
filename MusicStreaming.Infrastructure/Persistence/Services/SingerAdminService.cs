using MusicStreaming.Application.Common.Dtos.Admin;
using MusicStreaming.Application.Common.Exceptions;
using MusicStreaming.Application.Common.Interfaces.Repositories;
using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Services
{
    public class SingerAdminService : ISingerAdminService
    {
        private readonly IGenericRepository<Singer> _singerRepository;
        private readonly IGenericRepository<Song> _songRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDashboardNotifier _dashboardNotifier; 

        public SingerAdminService(
            IGenericRepository<Singer> singerRepository,
            IGenericRepository<Song> songRepository,
            IUnitOfWork unitOfWork,
            IDashboardNotifier dashboardNotifier) 
        {
            _singerRepository = singerRepository;
            _songRepository = songRepository;
            _unitOfWork = unitOfWork;
            _dashboardNotifier = dashboardNotifier;
        }

        public async Task<IEnumerable<SingerAdminDto>> GetAllSingersAsync(CancellationToken cancellationToken = default)
        {
            var singers = await _singerRepository.GetAllAsync();

            var activeSingers = singers.Where(s => !s.IsDeleted);

            var dtos = activeSingers.Select(s => new SingerAdminDto
            {
                Id = s.Id,
                Name = s.Name,
                AvatarUrl = s.AvatarUrl,
                Bio = s.Bio
            });

            return dtos.ToList();
        }

        public async Task<SingerAdminDto> CreateSingerAsync(CreateSingerDto dto, CancellationToken cancellationToken = default)
        {

            string avatarUrl = !string.IsNullOrEmpty(dto.AvatarUrl) ? dto.AvatarUrl : string.Empty;

            var singer = new Singer(
                name: dto.Name,
                avatarUrl: avatarUrl,
                bio: dto.Bio
            );

            await _singerRepository.AddAsync(singer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _dashboardNotifier.BroadcastDashboardUpdateAsync();

            return MapToDto(singer);
        }

        public async Task<SingerAdminDto> UpdateSingerAsync(UpdateSingerDto dto, CancellationToken cancellationToken = default)
        {
            var singer = await _singerRepository.GetByIdAsync(dto.Id);
            if (singer == null || singer.IsDeleted)
                throw new Exception($"Singer with Id {dto.Id} not found");

            string newAvatarUrl = !string.IsNullOrEmpty(dto.AvatarUrl) ? dto.AvatarUrl : singer.AvatarUrl;


            singer.UpdateInfo(dto.Name, newAvatarUrl, dto.Bio);

            _singerRepository.Update(singer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _dashboardNotifier.BroadcastDashboardUpdateAsync();

            return MapToDto(singer);
        }

        public async Task<bool> DeleteSingerAsync(int id, CancellationToken cancellationToken = default)
        {
            var singer = await _singerRepository.GetByIdAsync(id);
            if (singer == null || singer.IsDeleted) return false;

            bool hasSongs = await _songRepository.ExistsAsync(s => s.SingerId == id && !s.IsDeleted);
            if (hasSongs) throw new BadRequestException("Không thể xóa ca sĩ đang có bài hát trong hệ thống.");

            singer.Delete();

            _singerRepository.Update(singer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _dashboardNotifier.BroadcastDashboardUpdateAsync();

            return true;
        }

        private SingerAdminDto MapToDto(Singer singer)
        {
            return new SingerAdminDto
            {
                Id = singer.Id,
                Name = singer.Name,
                AvatarUrl = singer.AvatarUrl,
                Bio = singer.Bio
            };
        }

    }
}