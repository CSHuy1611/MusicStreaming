using MusicStreaming.Application.Common.Dtos.Admin;
using MusicStreaming.Application.Common.Exceptions;
using MusicStreaming.Application.Common.Interfaces.Repositories;
using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Services
{
    public class SubscriptionAdminService : ISubscriptionAdminService
    {
        private readonly IGenericRepository<SubscriptionPackage> _subscriptionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDashboardNotifier _dashboardNotifier; 

        public SubscriptionAdminService(
            IGenericRepository<SubscriptionPackage> subscriptionRepository,
            IUnitOfWork unitOfWork,
            IDashboardNotifier dashboardNotifier)
        {
            _subscriptionRepository = subscriptionRepository;
            _unitOfWork = unitOfWork;
            _dashboardNotifier = dashboardNotifier;
        }

        public async Task<IEnumerable<SubscriptionAdminDto>> GetAllSubscriptionsAsync(CancellationToken cancellationToken = default)
        {
            var packages = await _subscriptionRepository.GetAllAsync();

            var dtos = packages.Where(p => !p.IsDeleted).Select(p => new SubscriptionAdminDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                DurationInMonths = p.DurationInMonths,
                IsActive = p.IsActive
            });

            return dtos.ToList();
        }

        public async Task<SubscriptionAdminDto> CreateSubscriptionAsync(CreateSubscriptionDto dto, CancellationToken cancellationToken = default)
        {
            bool isDuplicate = await _subscriptionRepository.ExistsAsync(p => p.Name == dto.Name && !p.IsDeleted);
            if (isDuplicate)
            {
                throw new BadRequestException($"Gói VIP với tên '{dto.Name}' đã tồn tại trong hệ thống.");
            }


            var package = new SubscriptionPackage(
                name: dto.Name,
                price: dto.Price,
                durationInMonths: dto.DurationInMonths
            );


            if (!dto.IsActive)
            {
                package.Deactivate();
            }

            await _subscriptionRepository.AddAsync(package);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _dashboardNotifier.BroadcastDashboardUpdateAsync();

            return MapToDto(package);
        }

        public async Task<SubscriptionAdminDto> UpdateSubscriptionAsync(UpdateSubscriptionDto dto, CancellationToken cancellationToken = default)
        {
            var package = await _subscriptionRepository.GetByIdAsync(dto.Id);
            if (package == null || package.IsDeleted)
                throw new Exception($"Subscription Package with Id {dto.Id} not found");

            bool isDuplicate = await _subscriptionRepository.ExistsAsync(p => p.Id != dto.Id && p.Name == dto.Name && !p.IsDeleted);
            if (isDuplicate)
            {
                throw new BadRequestException($"Gói VIP với tên '{dto.Name}' đã tồn tại trong hệ thống.");
            }


            package.UpdateFullInfo(dto.Name, dto.Price, dto.DurationInMonths, dto.IsActive);

            _subscriptionRepository.Update(package);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _dashboardNotifier.BroadcastDashboardUpdateAsync();

            return MapToDto(package);
        }

        public async Task<bool> DeleteSubscriptionAsync(int id, CancellationToken cancellationToken = default)
        {
            var package = await _subscriptionRepository.GetByIdAsync(id);
            if (package == null || package.IsDeleted) return false;

            package.Delete();

            _subscriptionRepository.Update(package);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _dashboardNotifier.BroadcastDashboardUpdateAsync();

            return true;
        }

        private SubscriptionAdminDto MapToDto(SubscriptionPackage package)
        {
            return new SubscriptionAdminDto
            {
                Id = package.Id,
                Name = package.Name,
                Price = package.Price,
                DurationInMonths = package.DurationInMonths,
                IsActive = package.IsActive
            };
        }
    }
}
