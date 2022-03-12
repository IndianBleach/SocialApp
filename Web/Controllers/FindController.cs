using ApplicationCore.Entities.IdeaEntity;
using ApplicationCore.Identity;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebUi.Controllers.Extensions;
using WebUi.Models;

namespace WebUi.Controllers
{
    //cleaned
    public class FindController : ExtendedController
    {
        private readonly ITagService _tagService;
        private readonly IGlobalService<ApplicationUser> _globalService;
        private readonly IUserRepository _userRepository;

        public FindController(
            ITagService tagService,
            IGlobalService<ApplicationUser> globalService,
            IUserRepository userRepository)
        {
            _tagService = tagService;
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
            
            return View(new FindUsersViewModelUpdated(                
                await _globalService.GetLastNewsAsync(),
                await _userRepository.GetRecommendsUsersOrNullAsync(userGuid),
                _userRepository.GetUserListPerPage(page, search, tag, country, city),
                await _tagService.GetAllTagsAsync(),
                IsUserAuthenticated(),
                search,
                tag,
                country,
                city,
                _globalService.CreateSearchResultString(tag, search, country, city)));
        }
    }
}
