using MediatR;
using Microsoft.AspNetCore.Mvc;
using MusicStreaming.Application.Features.Home.Queries.GetHomeData;
using MusicStreaming.Web.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace MusicStreaming.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMediator _mediator; 
        public HomeController(ILogger<HomeController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy UserId từ Claims (nếu đã đăng nhập)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Truyền UserId vào Query
            var query = new GetHomeDataQuery { UserId = userId };

            var viewModel = await _mediator.Send(query);
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
