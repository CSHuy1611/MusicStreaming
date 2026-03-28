using MediatR;
using MusicStreaming.Application.Common.Interfaces;
using MusicStreaming.Application.Common.Interfaces.Repositories;
using MusicStreaming.Application.Common.Interfaces.Services;
using MusicStreaming.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreaming.Application.Features.Subscriptions.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, string>
    {
        private readonly IGenericRepository<SubscriptionPackage> _packageRepository;
        private readonly IOrderRepository _orderRepository;
 
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;


        public CreateOrderCommandHandler(
            IGenericRepository<SubscriptionPackage> packageRepository,
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork,
            IPaymentService paymentService)
        {
            _packageRepository = packageRepository;
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }

        public async Task<string> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy thông tin gói VIP từ DB
            var package = await _packageRepository.GetByIdAsync(request.PackageId);
            if (package == null || !package.IsActive)
            {
                return string.Empty; 
            }

            // 2. Tạo Order Code (PayOS yêu cầu số nguyên long tối đa 53 bit, Javascript safe)
            long orderCode = long.Parse(DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString().Substring(0, 10) + new Random().Next(1000, 9999).ToString());

            // 3. Tạo Order 
            var order = new Order(request.UserId, request.PackageId, package.Price, orderCode);
            await _orderRepository.AddAsync(order);

            // 4. Lưu vào DB trước khi tạo link thanh toán (Trạng thái Pending)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 5. Tạo link thanh toán qua PayOS
            var returnUrl = "http://localhost:5244/PayOS/Success";
            var cancelUrl = "http://localhost:5244/PayOS/Cancel";

            var checkoutUrl = await _paymentService.CreatePaymentLinkAsync(
                orderCode, 
                package.Name, 
                package.Price, 
                returnUrl, 
                cancelUrl);

            return checkoutUrl;
        }

    }
}
