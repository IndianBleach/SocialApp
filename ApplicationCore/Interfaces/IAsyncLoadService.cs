using ApplicationCore.DTOs.AsyncLoad;
using ApplicationCore.DTOs.AsyncLoad.Chat;
using ApplicationCore.Entities.Chat;
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
