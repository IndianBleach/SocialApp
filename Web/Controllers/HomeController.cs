using ApplicationCore.Entities.IdeaEntity;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using WebUi.Controllers.Extensions;
using WebUi.Models;

namespace WebUi.Controllers
{
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

                
        public async Task<IActionResult> Index(int? page)
        {


            HomeIdeasViewModel indexVm = new()
            {
                IsAuthorized = IsUserAuthenticated(),
                Tags = await _tagService.GetAllTagsAsync(),
                Ideas = _ideaRepository.GetIdeasPerPage(page, GetUserIdOrNull()),
                LastNews = await _globalService.GetLastNewsAsync(),
                Pages = _globalService.CreatePages(page),
                RecommendIdeas = _ideaRepository.GetRecommendIdeas(GetUserIdOrNull()),
                SearchReactions = await _globalService.GetSearchReactionsAsync()                
            };

            return View("index2", indexVm);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}