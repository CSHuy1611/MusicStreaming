using MusicStreaming.Application.Common.Dtos.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Interfaces.Services
{
    public interface ISubscriptionAdminService
    {
        Task<IEnumerable<SubscriptionAdminDto>> GetAllSubscriptionsAsync(CancellationToken cancellationToken = default);
        Task<SubscriptionAdminDto> CreateSubscriptionAsync(CreateSubscriptionDto dto, CancellationToken cancellationToken = default);
        Task<SubscriptionAdminDto> UpdateSubscriptionAsync(UpdateSubscriptionDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteSubscriptionAsync(int id, CancellationToken cancellationToken = default);
    }
}
