using ApplicationCore.Entities.IdeaEntity;
using ApplicationCore.Identity;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebUi.Controllers.Extensions;
using WebUi.Models;

namespace WebUi.Controllers
{
    public class FindController : ExtendedController
    {
        private readonly ITagService _tagService;
        private readonly IIdeaRepository _ideaRepository;
        private readonly IGlobalService<ApplicationUser> _globalService;
        private readonly IUserRepository _userRepository;

        public FindController(
            ITagService tagService,
            IIdeaRepository ideaRepository,
            IGlobalService<ApplicationUser> globalService,
            IUserRepository userRepository)
        {
            _tagService = tagService;
            _ideaRepository = ideaRepository;
            _globalService = globalService;
            _userRepository = userRepository;
        }

        [Route("/find")]
        public async Task<IActionResult> Index(int? page, string? search, string? country, string? city, string? tag)
        {            
            string userGuid = GetUserIdOrNull();

            if (!string.IsNullOrWhiteSpace(search))
                tag = null;
            if (!string.IsNullOrWhiteSpace(tag))
                search = null;

            FindUsersViewModel indexVm = new()
            {
                IsAuthorized = IsUserAuthenticated(),
                LastNews = await _globalService.GetLastNewsAsync(),
                Pages = _globalService.CreatePages(page),
                RecommendUsers = await _userRepository.GetRecommendsUsersOrNullAsync(userGuid),
                Tags = await _tagService.GetAllTagsAsync(),
                Users = _userRepository.GetUsersPerPage(page, search, tag, country, city),
                Search = search,
                SearchCity = city,
                SearchCountry = country,
                SearchTag = tag,
                SearchParamsResultString = _globalService
                    .CreateSearchResultString(tag, search, country, city)
            };

            return View(indexVm);
        }
    }
}
