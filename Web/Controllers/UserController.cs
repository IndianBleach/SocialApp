using ApplicationCore.Entities.IdeaEntity;
using ApplicationCore.Entities.UserEntity;
using ApplicationCore.Identity;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebUi.Controllers.Extensions;
using WebUi.Models.UserViewModel;

namespace WebUi.Controllers
{
    public class UserController : ExtendedController
    {
        private readonly ITagService _tagService;
        private readonly IIdeaRepository _ideaRepository;
        private readonly IGlobalService<ApplicationUser> _globalService;
        private readonly IUserRepository _userRepository;

        public UserController(
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

        [Route("user/{userGuid}")]
        public async Task<IActionResult> Index(string userGuid, int? page, string? section)
        {

            string? currentUserGuid = GetUserIdOrNull();

            if (userGuid == null)
                return RedirectToAction("login", "account");

            ProfileIndexViewModel indexVm = new()
            {
                Ideas = _userRepository.GetUserIdeasPerPage(page, userGuid),
                IsAuthorized = IsUserAuthenticated(),
                Tags = await _tagService.GetAllTagsAsync(),
                User = await _userRepository.GetUserDetailOrNullAsync(userGuid),
                Pages = _globalService.CreatePages(
                    _userRepository.GetUserIdeasCountByRole(userGuid, IdeaMemberRoles.Author)),
                FriendType = currentUserGuid != null ? 
                    await _userRepository.CheckFriendsAsync(currentUserGuid, userGuid) :
                    ProfileFriendshipType.NotFriends,
                IsSelfProfile = currentUserGuid != null ?
                    _userRepository.CheckIsSelfProfile(currentUserGuid, userGuid) :
                    false
            };

            return View(indexVm);
        }


    }
}
