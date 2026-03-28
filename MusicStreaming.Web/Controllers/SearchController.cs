using MediatR;
using Microsoft.AspNetCore.Mvc;
using MusicStreaming.Application.Features.Search.Queries.Explore;
using MusicStreaming.Application.Features.Search.Queries.SearchSongs;

namespace MusicStreaming.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly IMediator _mediator;

        public SearchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string keyword)
        {
            var query = new SearchSongsQuery { Keyword = keyword };
            var viewModel = await _mediator.Send(query);

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Explore([FromQuery] ExploreQuery query)
        {
            var viewModel = await _mediator.Send(query);

            ViewBag.Keyword = query.Keyword;
            ViewBag.GenreId = query.GenreId;
            ViewBag.SingerId = query.SingerId;
            ViewBag.SortBy = query.SortBy;

            return View(viewModel);
        }
    }
}
