﻿using ApplicationCore.DTOs.Idea;
using ApplicationCore.DTOs.User;
using ApplicationCore.Entities.IdeaEntity;
using ApplicationCore.Entities.UserEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IUserRepository
    {
        Task<UserProfileIdeaList> GetUserParticipationIdeaListAsync(string userId, string currentUserId, bool onlyAuthorIdeas, int? page);
        //
        Task<ActiveChatUserDto> GetChatUserOrNullAsync(string? userGuid, string currentUserGuid);
        Task<ICollection<ChatUserDto>> GetUserChatsAsync(string userGuid);
        Task<ProfileFriendshipType> CheckFriendsAsync(string currentUserGuid, string userProfileGuid);
        bool CheckIsSelfProfile(string currentUserGuid, string userProfileGuid);
        int GetUserIdeasCountByRole(string userGuid, IdeaMemberRoles role);
        ICollection<HomeIdeaDto> GetUserIdeasPerPage(int? page, string userGuid);
        Task<UserDetailDto?> GetUserDetailOrNullAsync(string userGuid); 
        ICollection<UserDto> GetUsersPerPage(int? page, string? search, string? tag, string? country, string? city);
        Task<ICollection<RecommendUserDto>?> GetRecommendsUsersOrNullAsync(string? userGuid);
    }
}
