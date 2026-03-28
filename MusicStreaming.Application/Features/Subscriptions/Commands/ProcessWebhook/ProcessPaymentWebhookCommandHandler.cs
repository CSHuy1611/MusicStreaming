using MediatR;
using MusicStreaming.Application.Common.Interfaces;
using MusicStreaming.Application.Common.Interfaces.Repositories;
using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.Domain.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Subscriptions.Commands.ProcessWebhook
{
    public class ProcessPaymentWebhookCommandHandler : IRequestHandler<ProcessPaymentWebhookCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IGenericRepository<SubscriptionPackage> _packageRepository;
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;
        private readonly IDashboardNotifier _dashboardNotifier;

        public ProcessPaymentWebhookCommandHandler(
            IOrderRepository orderRepository,
            IGenericRepository<SubscriptionPackage> packageRepository,
            IIdentityService identityService,
            IUnitOfWork unitOfWork,
            IPaymentService paymentService,
            IDashboardNotifier dashboardNotifier)
        {
            _orderRepository = orderRepository;
            _packageRepository = packageRepository;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            _dashboardNotifier = dashboardNotifier;
        }

        public async Task<bool> Handle(ProcessPaymentWebhookCommand request, CancellationToken cancellationToken)
        {
            // 1. Xác thực dữ liệu Webhook (Tránh giả mạo)
            if (!_paymentService.VerifyWebhookData(request.Data, request.Signature))
            {
                return false;
            }

            // 2. Tìm Order theo OrderCode
            var order = await _orderRepository.GetByOrderCodeAsync(request.OrderCode, cancellationToken);

            if (order == null || order.Status != Domain.Enums.OrderStatus.Pending)
            {
                return false;
            }

            // 3. Xử lý theo trạng thái từ PayOS
            if (request.Status == "PAID")
            {

                // Lấy thông tin gói để biết thời hạn nâng cấp
                var package = await _packageRepository.GetByIdAsync(order.PackageId);
                if (package != null)
                {
                    var isUpgraded = await _identityService.UpgradeVipAsync(order.UserId, package.DurationInMonths);
                    if (!isUpgraded)
                    {
                        return false;
                    }
                }
                order.MarkAsPaid();
            }
            else
            {
                order.MarkAsCancelled();
            }

            // 4. Lưu thay đổi
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _dashboardNotifier.BroadcastDashboardUpdateAsync();

            return true;
        }
    }
}
