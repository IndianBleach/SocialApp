using ApplicationCore.DTOs.User;
using ApplicationCore.Entities.IdeaEntity;
using ApplicationCore.Entities.UserEntity;
using ApplicationCore.Identity;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Web.Models.UserViewModel;
using WebUi.Controllers.Extensions;
using WebUi.Models.UserViewModel;

namespace WebUi.Controllers
{
    [Authorize]
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
        [Route("/user/get/participation")]
        public JsonResult GetUserParticipation()
        {
            string curUserId = GetUserIdOrNull();
            if (curUserId != null)
                return Json(_userRepository.GetUserParticipations(curUserId));
            else
                return Json(null);
        }


        [HttpPost]
        [Route("/user/im/general")]
        [ActionName("UpdateGeneral")]
        public async Task<IActionResult> UpdateGeneral(UpdateGeneralSettingsDto model)
        {
            string curUserId = GetUserIdOrNull();
            if (curUserId != null)
            {
                 await _userRepository.UpdateGeneralSettingsAsync(curUserId, model, User.Identities);
            }

            return RedirectToAction("Im", "User", new { section = "editgeneral"});
        }


        [HttpPost]
        [Route("/user/im/account")]
        public async Task<JsonResult> UpdateAccount(UpdateAccountSettingsDto model)
        {
            string curUserId = GetUserIdOrNull();
            if (curUserId != null)
            {
                var res = await _userRepository.UpdateAccountSettingsAsync(curUserId, model);

                return Json(res);
            }

            return Json(null);
        }


        [HttpGet]
        [Route("/user/im")]
        public async Task<IActionResult> SelfProfile(string? section)
        {
            string? currentUserGuid = GetUserIdOrNull();

            if (currentUserGuid == null)
                return RedirectToAction("login", "account");

            string[] allSections = new string[]
            {
                "editgeneral",
                "editaccount",
            };

            string validSection = allSections.Any(x => x.Equals(section?.ToLower())) == true
                ? section?.ToLower() : "";

            if (validSection.Equals("editgeneral"))
            {
                ProfileEditGeneralViewModel editVm = new()
                {
                    AllTags = await _tagService.GetAllTagsAsync(),
                    UserDetail = await _userRepository.GetEditGeneralUserAsync(currentUserGuid),
                };

                return View("EditGeneral", editVm);
            }
            else if (validSection.Equals("editaccount"))
            {
                ProfileEditAccountViewModel editVm = new()
                {
                    AllTags = await _tagService.GetAllTagsAsync(),
                    UserDetail = await _userRepository.GetEditAccountUserAsync(currentUserGuid),
                };

                return View("EditAccount", editVm);
            }
            else
            {
                ProfileIdeasViewModel indexVm = new()
                {
                    AuthoredIdeas = validSection.Equals("authored"),
                    Section = validSection,
                    UserDetail = await _userRepository.GetUserDetailOrNullAsync(currentUserGuid),
                    FriendType = currentUserGuid != null ?
                        await _userRepository.CheckFriendsAsync(currentUserGuid, currentUserGuid) :
                        ProfileFriendshipType.NotFriends,
                    AllTags = await _tagService.GetAllTagsAsync(),
                    IdeaList = await _userRepository.GetUserParticipationIdeaListAsync(
                        currentUserGuid, currentUserGuid, validSection.Equals("authored"), null),
                    IsAuthorized = IsUserAuthenticated(),
                    IsSelfProfile = currentUserGuid != null && _userRepository.CheckIsSelfProfile(currentUserGuid, currentUserGuid)
                };

                return View("Index", indexVm);
            }            
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

            if ((userGuid == null) || (currentUserGuid == null))
                return RedirectToAction("login", "account");

            string[] allSections = new string[]
            {
                "authored",
                "participation",
                "about",
            };

            string validSection = allSections.Any(x => x.Equals(section?.ToLower())) == true
                ? section?.ToLower() : "participation";

            if (validSection.Equals("about"))
            {
                ProfileAboutViewModel aboutVm = new()
                {
                    UserInfoDetail = await _userRepository.GetUserAboutInfoAsync(userGuid),
                    Section = validSection,
                    AllTags = await _tagService.GetAllTagsAsync(),
                    IsAuthorized = IsUserAuthenticated(),
                    IsSelfProfile = currentUserGuid != null && _userRepository.CheckIsSelfProfile(currentUserGuid, userGuid),
                    FriendType = currentUserGuid != null ?
                    await _userRepository.CheckFriendsAsync(currentUserGuid, userGuid) :
                    ProfileFriendshipType.NotFriends,
                };

                return View("About", aboutVm);
            }
            else
            {
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

                return View("Index", indexVm);
            }            
        }
    }
}
