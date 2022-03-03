using ApplicationCore.ChatHub;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using WebUi.Controllers.Extensions;
using WebUi.Models.ChatViewModel;

namespace WebUi.Controllers
{
    [Authorize]
    public class ChatController : ExtendedController
    {
        private readonly ITagService _tagService;
        private readonly IHubContext<ChatHub> _chatContext;
        private readonly IUserRepository _userRepository;

        public ChatController(ITagService tagService,
            IIdeaRepository ideaRepository,
            IUserRepository userRepository,
            IHubContext<ChatHub> chatContext,
            IAsyncLoadService loadService)
        {
            _tagService = tagService;
            _userRepository = userRepository;
            _chatContext = chatContext;
        }

        
        [HttpPost]
        [Route("chat/join/{connectionId}/{chatGuid}")]
        public async Task<IActionResult> Join(string connectionId, string? chatGuid)
        {
            await _chatContext.Groups.AddToGroupAsync(connectionId, chatGuid);

            return Ok();
        }

        [Route("/chat")]
        public async Task<IActionResult> Index(string? user)
        {
            string curUserGuid = GetUserIdOrNull();

            if (curUserGuid == null)
                return RedirectToAction("login", "account");

            ChatIndexViewModel indexVm = new()
            {
                Tags = await _tagService.GetAllTagsAsync(),
                IsAuthorized = IsUserAuthenticated(),
                Guid = curUserGuid,
                ChatUsers = await _userRepository.GetUserChatsAsync(curUserGuid),
                ActiveChatUser = await _userRepository.GetChatUserOrNullAsync(user, curUserGuid),                
            };

            return View(indexVm);
        }
    }
}
