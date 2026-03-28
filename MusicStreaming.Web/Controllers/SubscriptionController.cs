using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStreaming.Application.Features.Subscriptions.Commands.CreateOrder;
using MusicStreaming.Application.Features.Subscriptions.Queries.GetActivePackages;
using System.Security.Claims;

namespace MusicStreaming.Web.Controllers
{
    [Authorize] 
    public class SubscriptionController : Controller
    {
        private readonly IMediator _mediator;

        public SubscriptionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var query = new GetActivePackagesQuery();
            var packages = await _mediator.Send(query);

            return View(packages);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(int packageId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện thanh toán!" });
            }

            var command = new CreateOrderCommand
            {
                UserId = userId,
                PackageId = packageId
            };

            try
            {
                var result = await _mediator.Send(command);

                if (!string.IsNullOrEmpty(result))
                {
                    return Json(new { success = true, checkoutUrl = result });
                }

                return Json(new { success = false, message = "Có lỗi xảy ra trong quá trình xử lý giao dịch. Vui lòng thử lại!" });
            }
            catch (Exception ex)
            {
                // Giữ nguyên định dạng Json trả về để JS không bị lỗi parse ParseJSON
                return Json(new { success = false, message = $"Lỗi thanh toán: {ex.Message}" });
            }
        }
    }
}
