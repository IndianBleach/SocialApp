using ApplicationCore.DTOs.AsyncLoad;
using ApplicationCore.DTOs.User;
using ApplicationCore.Entities.IdeaEntity;
using ApplicationCore.Identity;
using ApplicationCore.Interfaces.Repositories;
using Infrastructure.Constants;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private ApplicationContext _dbContext;
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<ApplicationUser> _userManager;

        public AdminRepository(ApplicationContext ctx, 
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _dbContext = ctx;
            _roleManager = roleManager;
            _userManager = userManager;
        }



        public Task<OperationResultDto> CleanGoalTasksAsync(string currentUserId)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResultDto> CleanTopicCommentsAsync(string currentUserId)
        {
            throw new NotImplementedException();
        }

        public async Task<UserSmallDto> GetAdminAccountOrNullAsync(string userId)
        {
            ApplicationUser getUser = await _dbContext.Users
                .Include(x => x.Avatar)
                .FirstOrDefaultAsync(x => x.Id.Equals(userId));

            if (getUser != null)
            {
                ICollection<string> userRoles = await _userManager
                    .GetRolesAsync(getUser);

                bool isAdmin = userRoles.Any(x => x.Equals("admin"));
                if (isAdmin)
                {
                    return new UserSmallDto(getUser.Id, getUser.UserName, getUser.Avatar.Name);
                }
            }
            return null;
        }

        public async Task<OperationResultDto> RemoveAccountByIdAsync(string userId, string currentUserId)
        {
            if (GetAdminAccountOrNullAsync(currentUserId) != null)
            {
                try
                {
                    ApplicationUser getUser = await _dbContext.Users
                    .Include(x => x.Chats)
                    .ThenInclude(x => x.Chat)
                    .ThenInclude(x => x.Messages)
                    .Include(x => x.Avatar)
                    .Include(x => x.Contacts)
                    .Include(x => x.FriendRequests)
                    .Include(x => x.Friends)
                    .Include(x => x.Invitations)
                    .FirstOrDefaultAsync(x => x.Id.Equals(userId));

                    if (getUser != null)
                    {
                        _dbContext.Users.Remove(getUser);
                        await _dbContext.SaveChangesAsync();
                        return new(true, "Op. remove account by admin (Success)");
                    }
                }
                catch (Exception exp)
                {
                    return new(false, exp.Message);
                }
            }
            return new(false, "Op. remove account by admin (Failed)");
        }

        public async Task<OperationResultDto> RemoveGoalByIdAsync(string goalId, string currentUserId)
        {
            if (GetAdminAccountOrNullAsync(currentUserId) != null)
            {
                IdeaGoal getGoal = await _dbContext.IdeaGoals
                    .Include(x => x.Tasks)
                    .FirstOrDefaultAsync(x => x.Id.Equals(goalId));

                if (getGoal != null)
                {
                    _dbContext.IdeaGoals.Remove(getGoal);
                    await _dbContext.SaveChangesAsync();
                    return new(true, "Op. remove goal by admin (Success)");
                }
            }
            return new(false, "Op. remove goal by admin (Failed)");
        }

        public async Task<OperationResultDto> RemoveIdeaByIdAsync(string ideaId, string currentUserId)
        {
            if (GetAdminAccountOrNullAsync(currentUserId) != null)
            {
                Idea loadFullIdea = await _dbContext.Ideas
                    .Include(x => x.Reactions)
                    .Include(x => x.Members)
                    .Include(x => x.Invitations)
                    .Include(x => x.Reposts)
                    .Include(x => x.Topics)
                    .ThenInclude(x => x.Comments)
                    .Include(x => x.Goals)
                    .ThenInclude(x => x.Tasks)
                    .Include(x => x.Avatar)
                    .Include(x => x.Contact)
                    .FirstOrDefaultAsync(x => x.Id.Equals(ideaId));

                _dbContext.Ideas.Remove(loadFullIdea);
                await _dbContext.SaveChangesAsync();

                if (loadFullIdea.Avatar.Name != AvatarInformation.IdeaDefaultAvatarName)
                    File.Delete("wwwroot/media/ideaAvatars/" + loadFullIdea.Avatar.Name);

                return new(true, "Op. Remove idea by admin (Success)");
            }
            return new(false, "Op. rmeove idea by admin (Failed)");
        }

        public async Task<OperationResultDto> RemoveTopicByIdAsync(string topicId, string currentUserId)
        {
            if (GetAdminAccountOrNullAsync(currentUserId) != null)
            {
                IdeaTopic getTopic = await _dbContext.IdeaTopics
                    .Include(x => x.Comments)
                    .FirstOrDefaultAsync(x => x.Id.Equals(topicId));

                if (getTopic != null)
                {
                    if (!getTopic.IsDefault && !getTopic.IsInit)
                    {
                        _dbContext.IdeaTopics.Remove(getTopic);
                        await _dbContext.SaveChangesAsync();
                        return new(true, "Op. topic remove by admin (Success)");
                    }
                    else return new(false, "Op. topic is init or default (Failed)");
                }
            }

            return new(false, "Op. remove topic by admin (Failed)");
        }
    }
}
