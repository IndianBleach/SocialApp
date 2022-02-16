﻿using ApplicationCore.Entities.UserEntity;
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


        public async Task<bool> Test(string u1, string u2)
        {
            var res = await _loadService.SendFriendRequestAsync(u1, u2);

            return true;
        }

        [HttpPost]
        [Route("/asyncload/chat/repostidea")]
        public async Task<JsonResult> RepostIdea(string user, string idea)
        {
            string userGuid = GetUserIdOrNull();

            if ((userGuid != null) && (idea != null) && (user != null))
            {
                var res = await _loadService.RepostIdeaAsync(user, idea, userGuid);

                return Json(res);
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

            bool auth = IsUserAuthenticated();

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
                var res = await _loadService.GetChatOrNullAsync(chatGuid, currentUserGuid);

                if (res != null)
                    return Json(res);
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
                var res = await _loadService.SendChatMessageAsync(message, currentUser, chatUser, chat);

                return Json(res);
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
                var res = await _loadService.LoadNewChatUsersAsync(userGuid);

                return Json(res);
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
                var res = await _loadService.LoadNewChatUsersAsync(userGuid);

                return Json(res);
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
                var res = await _loadService.AcceptFriendRequestAsync(guid);

                return Json(res);
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
                var res = await _loadService.RemoveFriendAsync(guid, currentUserGuid);

                return Json(res);
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
            var res = await _loadService.LoadUserFriendsAsync(userGuid);

            return Json(res);
        }

        [HttpGet]
        [Route("asyncload/user/getfriendrequests")]
        public async Task<JsonResult> GetUserFriendRequests(string userGuid)
        {
            var res = await _loadService.LoadUserFriendRequestsAsync(userGuid);

            return Json(res);
        }
        #endregion

        [HttpPost]
        [Route("asyncload/sendfriend")]
        public async Task<JsonResult> SendFriendRequest(string userGuid)
        {
            var author = GetUserIdOrNull();

            if ((author != null) && (author != userGuid))
            {
                var res = await _loadService.SendFriendRequestAsync(author, userGuid);
                return Json(res);
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
