using MediatR;
using Microsoft.AspNetCore.Mvc;
using MusicStreaming.Application.Features.Singers.Queries.GetArtistProfile;

namespace MusicStreaming.Web.Controllers
{
    public class SingerController : Controller
    {
        private readonly IMediator _mediator;

        public SingerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var query = new GetArtistProfileQuery { SingerId = id };

            var viewModel = await _mediator.Send(query);

            if (viewModel == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(viewModel);
        }
    }
}
