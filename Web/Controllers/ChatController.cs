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
    public class ChatController : ExtendedController
    {

        private readonly ITagService _tagService;
        private readonly IIdeaRepository _ideaRepository;
        private readonly IHubContext<ChatHub> _chatContext;
        private readonly IUserRepository _userRepository;

        public ChatController(ITagService tagService,
            IIdeaRepository ideaRepository,
            IUserRepository userRepository,
            IHubContext<ChatHub> chatContext)
        {
            _tagService = tagService;
            _ideaRepository = ideaRepository;
            _userRepository = userRepository;
            _chatContext = chatContext;
        }

        
        [HttpPost]
        [Route("chat/join/{connectionId}/{chatGuid}")]
        public async Task<IActionResult> Join(string connectionId, string chatGuid)
        {
            await _chatContext.Groups.AddToGroupAsync(connectionId, chatGuid);

            return Ok();
        }


        public async Task<IActionResult> Index()
        {
            string curUserGuid = GetUserIdOrNull();

            ChatIndexViewModel indexVm = new()
            {
                Tags = await _tagService.GetAllTagsAsync(),
                IsAuthorized = IsUserAuthenticated(),
                Guid = curUserGuid,
                ChatUsers = await _userRepository.GetUserChatsAsync(curUserGuid)
            };

            return View(indexVm);
        }
    }
}
