using System.Threading.Tasks;

namespace MusicStreaming.Application.Common.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentLinkAsync(long orderCode, string productName, decimal amount, string returnUrl, string cancelUrl);
        bool VerifyWebhookData(object data, string signature);
    }
}
