using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStreaming.Application.Features.Favorites.Commands.ToggleFavoriteSong;
using MusicStreaming.Application.Features.Favorites.Queries.CheckFavorite;
using MusicStreaming.Application.Features.Favorites.Queries.GetFavoriteSongs;
using System.Security.Claims;

namespace MusicStreaming.Web.Controllers
{
    [Authorize] 
    public class FavoriteController : Controller
    {
        private readonly IMediator _mediator;

        public FavoriteController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = new GetFavoriteSongsQuery { UserId = userId };

            var viewModel = await _mediator.Send(query);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(int songId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện chức năng này!" });
            }

            var command = new ToggleFavoriteSongCommand
            {
                UserId = userId,
                SongId = songId
            };

            var isLiked = await _mediator.Send(command);

            return Json(new { success = true, isLiked = isLiked });
        }

        [HttpGet]
        public async Task<IActionResult> CheckStatus(int songId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { isLiked = false });
            }

            var query = new CheckFavoriteQuery
            {
                UserId = userId,
                SongId = songId
            };

            var isLiked = await _mediator.Send(query);
            return Json(new { isLiked = isLiked });
        }
    }
}
