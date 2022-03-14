using ApplicationCore.DTOs.AsyncLoad.Idea;
using ApplicationCore.Entities.IdeaEntity;
using ApplicationCore.Entities.UserEntity;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebUi.Controllers.Extensions;

namespace WebUi.Controllers
{
    public class AsyncLoadController : ExtendedController
    {
        private readonly IAsyncLoadService _loadService;

        public AsyncLoadController(IAsyncLoadService loadService)
        {
            _loadService = loadService;
        }


        [HttpPost]
        [Route("/asyncload/idea/remove")]
        public async Task<JsonResult> RemoveIdea(string idea, string password)
        {
            string curUserId = GetUserIdOrNull();
            if (curUserId != null)
            {
                return Json(await _loadService.RemoveIdeaAsync(idea, curUserId, password));
            }
            return Json(null);
        }

        [HttpPost]
        [Route("/asyncload/idea/updaterole")]
        public async Task<JsonResult> IdeaRoleToDefault(string idea, string user, IdeaRolesToUpdate role)
        {
            string curUserId = GetUserIdOrNull();
            if (curUserId != null)
            {
                return Json(await _loadService.SetIdeaMemberRoleAsync(idea, user, curUserId, role));
            }
            return Json(null);
        }

        [HttpPost]
        [Route("/asyncload/idea/update")]
        public async Task<JsonResult> UpdateIdea(UpdateIdeaModel model)
        {
            string curUserId = GetUserIdOrNull();

            if ((curUserId != null) && ModelState.IsValid)
            {
                return Json(await _loadService.UpdateIdeaAsync(model, curUserId));
            }

            return Json(null);
        }

        [HttpPost]
        [Route("/asyncload/idea/createtask")]
        public async Task<JsonResult> CreateIdeaGoalTask(string content, string idea, string goal)
        {
            string curUserId = GetUserIdOrNull();
            if (curUserId != null)
            {
                return Json(await _loadService.CreateGoalTaskAsync(content, idea, goal, curUserId));
            }
            return Json(null);
        }

        [HttpPost]
        [Route("/asyncload/idea/changetask")]
        public async Task<JsonResult> ChangeIdeaGoalTask(IdeaGoalTaskType newStatus, string task, string goal)
        {
            string curUserId = GetUserIdOrNull();
            if (curUserId != null)
            {
                return Json(await _loadService.ChangeGoalTaskStatusAsync(curUserId, goal, task, newStatus));
            }
            return Json(null);
        }

        [HttpGet]
        [Route("/asyncload/idea/getgoal")]
        public async Task<JsonResult> GetIdeaGoalDetail(string goal)
        {
            string curUserId = GetUserIdOrNull();
            if (curUserId != null)
            {
                return Json(await _loadService.GetGoalDetailOrNullAsync(curUserId, goal));
            }
            return Json(null);
        }

        [HttpPost]
        [Route("/asyncload/idea/rejectmember")]
        public async Task<JsonResult> RejectIdeaMemberRequest(string idea, string user)
        {
            string curUserId = GetUserIdOrNull();
            if (curUserId != null)
            {
                return Json(await _loadService.RejectIdeaMemberRequestAsync(idea, user, curUserId));
            }
            return Json(null);
        }

        [HttpPost]
        [Route("/asyncload/idea/removemember")]
        public async Task<JsonResult> RemoveIdeaMember(string idea, string user)
        {
            string curUserId = GetUserIdOrNull();
            if (curUserId != null)
            {
                return Json(await _loadService.RemoveIdeaMemberAsync(idea, user, curUserId));
            }
            return Json(null);
        }

        [HttpPost]
        [Route("/asyncload/idea/acceptmember")]
        public async Task<JsonResult> AcceptIdeaMember(string idea, string user)
        {
            string curUserId = GetUserIdOrNull();
            if (curUserId != null)
            {
                return Json(await _loadService.AcceptIdeaMemberRequestAsync(idea, user, curUserId));
            }
            return Json(null);
        }

        [HttpPost]
        [Route("/asyncload/idea/creategoal")]
        public async Task<JsonResult> CreateGoalAsync(string idea, string name, string desc, bool withTasks)
        {
            string curUserGuid = GetUserIdOrNull();
            if (curUserGuid != null)
            {
                return Json(await _loadService.CreateGoalAsync(name, desc, idea, withTasks, curUserGuid));
            }
            return Json(null);
        }

        [HttpPost]
        [Route("/asyncload/idea/createtopic")]
        public async Task<JsonResult> CreateTopic(string name, string content, string ideaGuid)
        {
            return Json(await _loadService.CreateTopicAsync(name, content, ideaGuid, GetUserIdOrNull()));
        }

        [Route("/asyncload/idea/removetopiccomment")]
        public async Task<JsonResult> RemoveTopicComment(string topicGuid, string commentGuid)
        {
            string curUserGuid = GetUserIdOrNull();
            if (curUserGuid != null)
            {
                return Json(await _loadService.RemoveTopicCommentAsync(commentGuid, topicGuid, curUserGuid));
            }
            return Json(null);
        }

        [Route("/asyncload/idea/removetopic")]
        public async Task<JsonResult> RemoveTopic(string topicGuid)
        {
            string curUserGuid = GetUserIdOrNull();
            if (curUserGuid != null)
            {
                return Json(await _loadService.RemoveTopicAsync(topicGuid, curUserGuid));
            }
            return Json(null);
        }

        [HttpPost]
        [Route("/asyncload/idea/topiccomment")]
        public async Task<JsonResult> CreateTopicComment(string guid, string text)
        {
            string authorGuid = GetUserIdOrNull();
            if (authorGuid != null)
            {
                return Json(await _loadService.CreateTopicCommentAsync(authorGuid, guid, text));
            }
            return Json(null);
        }

        [HttpGet]
        [Route("/asyncload/idea/gettopic")]
        public async Task<JsonResult> GetTopicDetail(string guid)
        {
            string curUserGuid = GetUserIdOrNull();

            if (curUserGuid != null)
            {
                return Json(await _loadService.GetTopicDetailOrNullAsync(curUserGuid, guid));
            }

            return Json(null);
        }

        [HttpPost]
        [Route("/asyncload/user/sendinvite")]
        public async Task<JsonResult> SendUserInviteToIdea(string user, string idea)
        {
            return Json(await _loadService.SendIdeaInviteAsync(GetUserIdOrNull(), user, idea));
        }

        [HttpGet]
        [Route("/asyncload/user/getinvite")]
        public async Task<JsonResult> GetUserIdeasToInvite()
        {
            string curUserid = GetUserIdOrNull();
            if (curUserid != null)
            {
                return Json(_loadService.GetUserIdeasToInvite(curUserid));
            }

            return Json(null);
        }

        [HttpPost]
        [Route("/asyncload/chat/repostidea")]
        public async Task<JsonResult> RepostIdea(string user, string idea)
        {
            string userGuid = GetUserIdOrNull();

            if ((userGuid != null) && (idea != null) && (user != null))
            {
                return Json(await _loadService.RepostIdeaAsync(user, idea, userGuid));
            }

            return Json(null);
        }

        [HttpPost]
        [Route("/asyncload/idea/setreaction")]
        public async Task<JsonResult> IdeaSetReaction(string reaction, string idea)
        {
            string userId = GetUserIdOrNull();

            if ((userId != null) && 
                !string.IsNullOrEmpty(reaction) &&
                (IsUserAuthenticated() == true))
            {
                var res = await _loadService.IdeaSetReactionAsync(reaction, idea, userId);
                await _loadService.SaveChangesAsync();

                return Json(res);
            }
            else return Json(null);
        }

        [HttpPost]
        [Route("/asyncload/idea/setlike")]
        public async Task<JsonResult> IdeaSetLike(string idea)
        {
            string userId = GetUserIdOrNull();

            if ((userId != null) && IsUserAuthenticated())
            {
                var res = await _loadService.IdeaSendJoinRequestAsync(userId, idea);
                await _loadService.SaveChangesAsync();

                return Json(res);
            }
            else return Json(null);
        }

        [HttpGet]
        [Route("asyncload/chat/getchat")]
        public async Task<JsonResult> GetChat(string chatGuid)
        {
            string? currentUserGuid = GetUserIdOrNull();

            if (currentUserGuid != null)
            {
                return Json(await _loadService.GetChatOrNullAsync(chatGuid, currentUserGuid));
            }

            return Json(null);
        }

        [HttpPost]
        [Route("asyncload/chat/sendmessage")]
        public async Task<JsonResult> SendMessage(string message, string chatUser, string? chat)
        {
            string currentUser = GetUserIdOrNull();

            if (currentUser != null)
            {
                return Json(await _loadService.SendChatMessageAsync(message, currentUser, chatUser, chat));
            }

            return Json(null);
        }

        [HttpPost]
        [Route("asyncload/chat/create")]
        public async Task<JsonResult> CreateChat(string userGuid, string message)
        {
            string? curUserGuid = GetUserIdOrNull();

            if ((curUserGuid != null) && curUserGuid.Equals(userGuid))
            {
                return Json(await _loadService.LoadNewChatUsersAsync(userGuid));
            }

            return Json(null);
        }

        [HttpGet]
        [Route("asyncload/chat/new")]
        public async Task<JsonResult> GetNewChatUsers(string userGuid)
        {
            string? curUserGuid = GetUserIdOrNull();

            if ((curUserGuid != null) && curUserGuid == userGuid)
            {
                return Json(await _loadService.LoadNewChatUsersAsync(userGuid));
            }

            return Json(null);
        }

        #region /user

        [HttpPost]
        [Route("asyncload/user/acceptfriend")]
        public async Task<JsonResult> AcceptFriendRequest(string guid)
        {
            string? currentUserGuid = GetUserIdOrNull();

            if (currentUserGuid != null)
            {
                return Json(await _loadService.AcceptFriendRequestAsync(guid));
            }
            return Json(null);
        }


        [HttpPost]
        [Route("asyncload/user/removefriend")]
        public async Task<JsonResult> RemoveFriendship(string guid)
        {
            string? currentUserGuid = GetUserIdOrNull();

            if (currentUserGuid != null)
            {
                return Json(await _loadService.RemoveFriendAsync(guid, currentUserGuid));
            }
            return Json(null);
        }


        [HttpPost]
        [Route("asyncload/user/isme")]
        public async Task<JsonResult> CheckSelfProfile(string userGuid)
        { 
            string? currentUserGuid = GetUserIdOrNull();

            if ((currentUserGuid != null) && (currentUserGuid == userGuid))
                return Json(true);

            return Json(false);
        }

        [HttpGet]
        [Route("asyncload/user/getfriends")]
        public async Task<JsonResult> GetUserFriends(string userGuid)
        {
            return Json(await _loadService.LoadUserFriendsAsync(userGuid));
        }

        [HttpGet]
        [Route("asyncload/user/getfriendrequests")]
        public async Task<JsonResult> GetUserFriendRequests(string userGuid)
        {
            return Json(await _loadService.LoadUserFriendRequestsAsync(userGuid));
        }
        #endregion

        [HttpPost]
        [Route("asyncload/sendfriend")]
        public async Task<JsonResult> SendFriendRequest(string userGuid)
        {
            var author = GetUserIdOrNull();

            if ((author != null) && (author != userGuid))
            {
                return Json(await _loadService.SendFriendRequestAsync(author, userGuid));
            }
            return Json(null);            
        }

        [HttpGet]
        [Route("asyncload/repostusers")]
        public async Task<JsonResult> LoadRepostUsers()
        {
            string userGuid = GetUserIdOrNull();

            if (userGuid != null)
                return Json(await _loadService.LoadRepostUsersAsync(userGuid));

            return Json(null);
        }
    }
}
