using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStreaming.Application.Features.Library.Queries.GetUserPlaylists;
using System.Security.Claims;

namespace MusicStreaming.Web.Controllers
{
    [Authorize] 
    public class LibraryController : Controller
    {
        private readonly IMediator _mediator;

        public LibraryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Lấy ID người dùng đang đăng nhập
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 2. Gửi Query lấy danh sách
            var query = new GetUserPlaylistsQuery { UserId = userId };
            var playlists = await _mediator.Send(query);

            // 3. Trả về View
            return View(playlists);
        }
    }
}
