using MediatR;
using Microsoft.AspNetCore.Mvc;
using MusicStreaming.Application.Features.Songs.Commands.IncrementListen;
using MusicStreaming.Application.Features.Songs.Queries.GetAllSongs;
using System.Threading.Tasks;

namespace MusicStreaming.Web.Controllers
{
    public class SongController : Controller
    {
        private readonly IMediator _mediator;

        public SongController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 12)
        {
            var query = new GetAllSongsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var viewModel = await _mediator.Send(query);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> IncrementListen(int id)
        {
            var command = new IncrementListenCommand(id);
            var isSuccess = await _mediator.Send(command);

            if (isSuccess)
            {
                return Ok(new { success = true, message = "Tăng lượt nghe thành công." });
            }

            return NotFound(new { success = false, message = "Không tìm thấy bài hát hoặc lỗi xử lý." });
        }
    }
}