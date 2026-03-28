using Microsoft.Extensions.Configuration;
using MusicStreaming.Application.Common.Interfaces.Services;
using PayOS;
using PayOS.Models.V2.PaymentRequests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicStreaming.Infrastructure.Services
{
    public class PayOSService : IPaymentService
    {
        private readonly PayOSClient _payOS;

        public PayOSService(IConfiguration configuration)
        {
            var clientId = configuration["PayOS:ClientId"] ?? throw new ArgumentNullException("PayOS ClientId is missing");
            var apiKey = configuration["PayOS:ApiKey"] ?? throw new ArgumentNullException("PayOS ApiKey is missing");
            var checksumKey = configuration["PayOS:ChecksumKey"] ?? throw new ArgumentNullException("PayOS ChecksumKey is missing");

            _payOS = new PayOSClient(clientId, apiKey, checksumKey, "");
        }

        public async Task<string> CreatePaymentLinkAsync(long orderCode, string productName, decimal amount, string returnUrl, string cancelUrl)
        {
            var items = new List<PaymentLinkItem>  
            {
                new PaymentLinkItem { Name = productName, Quantity = 1, Price = (long)amount }
            };

            var request = new CreatePaymentLinkRequest 
            {
                OrderCode = orderCode,
                Amount = (long)amount,
                Description = $"Thanh toan {orderCode}",
                Items = items,
                CancelUrl = cancelUrl,
                ReturnUrl = returnUrl,
            };

            var result = await _payOS.PaymentRequests.CreateAsync(request);
            return result.CheckoutUrl;
        }

        public bool VerifyWebhookData(object data, string signature)
        {
            return true;
        }
    }
}
