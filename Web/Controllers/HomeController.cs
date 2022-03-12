using ApplicationCore.Entities.IdeaEntity;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebUi.Controllers.Extensions;
using WebUi.Models;

namespace WebUi.Controllers
{
    //cleaned
    public class HomeController : ExtendedController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITagService _tagService;
        private readonly IIdeaRepository _ideaRepository;
        private readonly IGlobalService<Idea> _globalService;

        public HomeController(ILogger<HomeController> logger,
            ITagService tagService,
            IIdeaRepository ideaRepository,
            IGlobalService<Idea> globalService)
        {
            _logger = logger;
            _tagService = tagService;
            _ideaRepository = ideaRepository;
            _globalService = globalService;
        }

        [AllowAnonymous]
        [Route("/notfound")]
        public IActionResult NotFound404()
            => View("notFound");

        [AllowAnonymous]
        [Route("/home")]
        public async Task<IActionResult> Index(int? page, string? react, string? key, string? tag, string? search)
        {
            if (!string.IsNullOrEmpty(tag) ||
                !string.IsNullOrEmpty(key) ||
                !string.IsNullOrEmpty(search))
                react = null;

            return View(new HomeIdeasViewModel(
                _globalService.CreatePages(page),
                await _globalService.GetLastNewsAsync(),
                _ideaRepository.GetRecommendIdeas(GetUserIdOrNull()),
                _ideaRepository.GetIdeasPerPage(page, GetUserIdOrNull(), react, key, tag, search),
                await _globalService.GetSearchReactionsAsync(),
                await _tagService.GetAllTagsAsync(),
                IsUserAuthenticated(),
                react,
                search,
                key,
                tag));
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}