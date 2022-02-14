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

        public async Task<IActionResult> Index(int? page)
        {
            string userGuid = GetUserIdOrNull();

            FindUsersViewModel indexVm = new()
            {
                IsAuthorized = IsUserAuthenticated(),
                LastNews = await _globalService.GetLastNewsAsync(),
                Pages = _globalService.CreatePages(page),    
                RecommendUsers = await _userRepository.GetRecommendsUsersOrNullAsync(userGuid),
                Tags = await _tagService.GetAllTagsAsync(),
                Users = _userRepository.GetUsersPerPage(page)
            };

            return View(indexVm);
        }
    }
}
