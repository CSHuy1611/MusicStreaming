using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStreaming.Application.Features.Library.Queries.GetUserPlaylists;
using MusicStreaming.Application.Features.Playlists.Commands.AddSongToPlaylist;
using MusicStreaming.Application.Features.Playlists.Commands.CreatePlaylist;
using MusicStreaming.Application.Features.Playlists.Queries.GetPlaylistDetail;
using System.Security.Claims;

namespace MusicStreaming.Web.Controllers
{
    public class PlaylistController : Controller
    {
        private readonly IMediator _mediator;

        public PlaylistController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: /Playlist/Detail/5
        public async Task<IActionResult> Detail(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            var query = new GetPlaylistDetailQuery { PlaylistId = id, UserId = userId }; 
            var viewModel = await _mediator.Send(query);
            if (viewModel == null)
            {
                return NotFound();
            }
            return View(viewModel);
        }

        [HttpPost]
        [Authorize] 
        public async Task<IActionResult> Create([FromForm]string playlistName)
        {
            if (string.IsNullOrWhiteSpace(playlistName))
            {
                return Json(new { success = false, message = "Tên Playlist không được để trống!" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var command = new CreatePlaylistCommand
            {
                UserId = userId!,
                Name = playlistName
            };

            var newPlaylistId = await _mediator.Send(command);

            return Json(new { success = true, message = "Tạo playlist thành công!", playlistId = newPlaylistId });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPlaylists()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = new GetUserPlaylistsQuery { UserId = userId };
            var playlists = await _mediator.Send(query);

            return Json(playlists);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddSongToPlaylist(int playlistId, int songId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var command = new AddSongToPlaylistCommand
            {
                PlaylistId = playlistId,
                SongId = songId,
                UserId = userId
            };

            var isSuccess = await _mediator.Send(command);

            if (isSuccess)
            {
                return Json(new { success = true, message = "Đã thêm vào Playlist thành công!" });
            }

            return Json(new { success = false, message = "Lỗi: Không tìm thấy Playlist hoặc bài hát đã tồn tại." });
        }
    }
}
