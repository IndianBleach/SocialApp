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
        private readonly IUserRepository _userRepository;

        public UserController(
            ITagService tagService,
            IIdeaRepository ideaRepository,
            IGlobalService<ApplicationUser> globalService,
            IUserRepository userRepository)
        {
            _tagService = tagService;
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
                var res = await _userRepository.UpdateAccountSettingsAsync(curUserId, model, User.Identities);

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
                UserEditGeneralDto getUser = await _userRepository.GetEditGeneralUserAsync(currentUserGuid);
                if (getUser == null)
                    return NotFound();

                ProfileEditGeneralViewModel editVm = new()
                {
                    AllTags = await _tagService.GetAllTagsAsync(),
                    UserDetail = getUser,
                };

                return View("EditGeneral", editVm);
            }
            else if (validSection.Equals("editaccount"))
            {
                UserEditAccountDto getUser = await _userRepository.GetEditAccountUserAsync(currentUserGuid);
                if (getUser == null)
                    return NotFound();

                ProfileEditAccountViewModel editVm = new()
                {
                    AllTags = await _tagService.GetAllTagsAsync(),
                    UserDetail = getUser,
                };

                return View("EditAccount", editVm);
            }
            else
            {
                UserDetailDto getUser = await _userRepository.GetUserDetailOrNullAsync(currentUserGuid);
                if (getUser == null)
                    return NotFound();

                ProfileIdeasViewModel indexVm = new()
                {
                    AuthoredIdeas = validSection.Equals("authored"),
                    Section = validSection,
                    UserDetail = getUser,
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

        [HttpGet]
        [Route("/user/{userGuid}")]
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
                UserAboutInfoDto getUser = await _userRepository.GetUserAboutInfoAsync(userGuid);
                if (getUser == null)
                    return NotFound();
                ProfileAboutViewModel aboutVm = new()
                {
                    UserInfoDetail = getUser,
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
                UserDetailDto getUser = await _userRepository.GetUserDetailOrNullAsync(userGuid);
                if (getUser == null)
                    return NotFound();

                ProfileIdeasViewModel indexVm = new()
                {
                    AuthoredIdeas = validSection.Equals("authored"),
                    Section = validSection,
                    UserDetail = getUser,
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
