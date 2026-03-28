using MediatR;
using Microsoft.AspNetCore.Mvc;
using MusicStreaming.Application.Common.Webhooks;
using MusicStreaming.Application.Features.Subscriptions.Commands.ProcessWebhook;
using System.Threading.Tasks;

namespace MusicStreaming.Web.Controllers
{
    [Route("[controller]")]
    public class PayOSController : Controller
    {
        private readonly IMediator _mediator;

        public PayOSController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Webhooks")]
        public async Task<IActionResult> Webhook([FromBody] WebhookRequest? request)
        {
            if (request == null || request.data == null) return Ok(new { success = true });

            var command = new ProcessPaymentWebhookCommand
            {
                OrderCode = request.data.orderCode,
                Status = request.code == "00" ? "PAID" : "CANCELLED",
                Signature = request.signature ?? string.Empty,
                Data = request.data
            };

            var result = await _mediator.Send(command);

            return Ok(new { success = result });
        }

        [HttpGet("Success")]
        public IActionResult Success()
        {
            return View();
        }

        [HttpGet("Cancel")]
        public IActionResult Cancel()
        {
            return View();
        }
    }



}
