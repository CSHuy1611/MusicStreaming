using MediatR;

namespace MusicStreaming.Application.Features.Subscriptions.Commands.ProcessWebhook
{
    public class ProcessPaymentWebhookCommand : IRequest<bool>
    {
        public long OrderCode { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Signature { get; set; } = string.Empty;
        public object? Data { get; set; }
    }
}
