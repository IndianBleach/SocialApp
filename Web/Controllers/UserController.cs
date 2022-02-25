using ApplicationCore.Entities.IdeaEntity;
using ApplicationCore.Entities.UserEntity;
using ApplicationCore.Identity;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Web.Models.UserViewModel;
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


        [HttpGet]
        [Route("/user/im")]
        public async Task<IActionResult> SelfProfile()
        { 
            return View();
        }


        public async Task<IActionResult> Test(string userGuid, int? page, string? section)
        {
            string? currentUserGuid = GetUserIdOrNull();

            string[] allSections = new string[]
            {
                "selfideas",
                "participation",
                "about",
                "edit",
            };

            string validSection = allSections.Any(x => x.Equals(section?.ToLower())) == true
                ? section?.ToLower() : "participation";

            if (userGuid == null)
                return RedirectToAction("login", "account");


            /*
            ProfileIdeasViewModel indexVm = new()
            {
                IdeaList = await _userRepository.GetUserParticipationIdeaListAsync(userGuid, currentUserGuid, page),
                UserDetail = await _userRepository.GetUserDetailOrNullAsync
            };
            */

            return View();
        }


        [HttpGet]
        [Route("user/{userGuid}")]
        public async Task<IActionResult> Index(string userGuid, int? page, string? section)
        {
            string? currentUserGuid = GetUserIdOrNull();

            if (userGuid == null)
                return RedirectToAction("login", "account");

            string[] allSections = new string[]
            {
                "authored",
                "participation",
                "about",
                "edit",
            };

            string validSection = allSections.Any(x => x.Equals(section?.ToLower())) == true
                ? section?.ToLower() : "participation";

            if (validSection.Equals("about"))
            { 
            
            }

            ProfileIdeasViewModel indexVm = new()
            {
                AuthoredIdeas = validSection.Equals("authored"),
                Section = validSection,
                UserDetail = await _userRepository.GetUserDetailOrNullAsync(userGuid),
                FriendType = currentUserGuid != null ?
                    await _userRepository.CheckFriendsAsync(currentUserGuid, userGuid) :
                    ProfileFriendshipType.NotFriends,
                AllTags = await _tagService.GetAllTagsAsync(),
                IdeaList = await _userRepository.GetUserParticipationIdeaListAsync(
                    userGuid, currentUserGuid, validSection.Equals("authored"), page),
                IsAuthorized = IsUserAuthenticated(),
                IsSelfProfile = currentUserGuid != null && _userRepository.CheckIsSelfProfile(currentUserGuid, userGuid)
            };

            return View("Index2", indexVm);
        }


    }
}
