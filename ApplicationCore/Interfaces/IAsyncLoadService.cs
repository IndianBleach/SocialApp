using ApplicationCore.DTOs.AsyncLoad;
using ApplicationCore.DTOs.AsyncLoad.Chat;
using ApplicationCore.DTOs.AsyncLoad.Idea;
using ApplicationCore.Entities.Chat;
using ApplicationCore.Entities.IdeaEntity;
using ApplicationCore.Entities.UserEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IAsyncLoadService
    {
        Task<OperationResultDto> ChangeGoalTaskStatusAsync(string currentUserGuid, string goalGuid, string taskGuid, IdeaGoalTaskType newStatus);
        Task<GoalDetailDto?> GetGoalDetailOrNullAsync(string currentUserGuid, string goalGuid);
        Task<OperationResultDto> CreateGoalAsync(string name, string description, string ideaGuid, bool withTasks, string authorGuid);
        Task<OperationResultDto> CreateGoalTaskAsync(string content, string ideaGuid, string goalGuid, string currentUserGuid);
        //
        Task<OperationResultDto> RemoveIdeaMemberAsync(string ideaGuid, string userGuid, string curUserGuid);
        Task<OperationResultDto> RejectIdeaMemberRequestAsync(string ideaGuid, string userGuid, string curUserGuid);
        Task<OperationResultDto> AcceptIdeaMemberRequestAsync(string ideaGuid, string userGuid, string curUserGuid);
        Task<OperationResultDto> CreateTopicAsync(string name, string content, string ideaGuid, string authorGuid);
        Task<OperationResultDto> RemoveTopicCommentAsync(string commentGuid, string topicGuid, string currentUserGuid);
        Task<OperationResultDto> RemoveTopicAsync(string topicGuid, string currentUserRole);
        //
        Task<OperationResultDto> CreateTopicCommentAsync(string authorGuid, string topicGuid, string text);
        Task<TopicDetail?> GetTopicDetailOrNullAsync(string currentUserGuid, string topicGuid);
        //
        Task<OperationResultDto> SendIdeaInviteAsync(string? currentUserGuid, string userGuid, string ideaGuid);
        IEnumerable<IdeaToInviteDto> GetUserIdeasToInvite(string userGuid);
        Task<OperationResultDto> RepostIdeaAsync(string userGuid, string ideaGuid, string currentUserGuid);
        Task<ChatDetailDto?> GetActiveChatOrNullAsync(string? userGuid, string? currentUserGuid);

        //
        Task SaveChangesAsync();
        Task<OperationResultDto> IdeaSendJoinRequestAsync(string userGuid, string ideaGuid);
        Task<OperationResultDto> IdeaSetReactionAsync(string reactionGuid, string ideaGuid, string userGuid);
       
        Task<MessageResultDto> SendChatMessageAsync(string message, string authorGuid, string userGuid, string? chatGuid);        
        Task<OperationResultDto> RemoveFriendAsync(string friendguid, string currentUserGuid);
        Task<OperationResultDto> AcceptFriendRequestAsync(string guid);
        Task<OperationResultDto> SendFriendRequestAsync(string userGuid, string friendGuid);
        Task<IEnumerable<RepostToUserDto>> LoadRepostUsersAsync(string userGuid);
        Task<IEnumerable<FriendUserDto>> LoadUserFriendsAsync(string userGuid);
        Task<IEnumerable<SmallUserDto>> LoadNewChatUsersAsync(string guid);
        Task<IEnumerable<FriendRequestDto>> LoadUserFriendRequestsAsync(string userGuid);
        Task<ChatDetailDto?> GetChatOrNullAsync(string chatGuid, string currentUserGuid);
        
    }
}
