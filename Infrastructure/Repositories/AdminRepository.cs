using ApplicationCore.DTOs.AsyncLoad;
using ApplicationCore.DTOs.User;
using ApplicationCore.Entities.Chat;
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

        public async Task<OperationResultDto> CleanGoalTasksAsync(string currentUserId)
        {
            if (GetAdminAccountOrNullAsync(currentUserId) != null)
            {
                ICollection<IdeaGoal> getTopics = await _dbContext.IdeaGoals
                    .Include(x => x.Tasks)
                    .ToListAsync();

                foreach (var goal in getTopics)
                {
                    _dbContext.IdeaGoalTasks
                        .RemoveRange(goal.Tasks
                            .OrderByDescending(x => x.Type.Equals(IdeaGoalTaskType.Complete))
                            .OrderByDescending(x => x.DateCreated)
                            .Skip(50));
                }

                await _dbContext.SaveChangesAsync();
                return new(true, "Op. clean goals comments by admin (Success)");
            }
            return new(true, "Op. clean goals comments by admin (Failed)");
        }

        public async Task<OperationResultDto> CleanTopicCommentsAsync(string currentUserId)
        {
            if (GetAdminAccountOrNullAsync(currentUserId) != null)
            {
                ICollection<IdeaTopic> getTopics = await _dbContext.IdeaTopics
                    .Include(x => x.Comments)
                    .ToListAsync();

                foreach (var topic in getTopics)
                {
                    _dbContext.IdeaTopicComments
                        .RemoveRange(topic.Comments
                            .OrderByDescending(x => x.DateCreated)
                            .Skip(40));
                }

                await _dbContext.SaveChangesAsync();
                return new(true, "Op. clean topic comments by admin (Success)");
            }
            return new(true, "Op. clean topic comments by admin (Failed)");
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
                    // 1) REMOVE ALL IDEAS (=Author)
                    List<Idea> getUserIdeas = await _dbContext.Ideas
                        .Include(x => x.Status)
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
                        .Where(x => x.Members.Any(x => x.UserId.Equals(userId) &&
                            x.Role.Equals(IdeaMemberRoles.Author)))
                        .ToListAsync();

                    if (getUserIdeas.Count > 0)
                    {
                        foreach (var idea in getUserIdeas)
                        {
                            if (!idea.Avatar.Name.Equals(AvatarInformation.IdeaDefaultAvatarName) &&
                                    !idea.Avatar.IsDefault)
                            {
                                File.Delete("wwwroot/media/ideaAvatars/" + idea.Avatar.Name);
                            }
                        }


                        _dbContext.Ideas.RemoveRange(getUserIdeas);
                        await _dbContext.SaveChangesAsync();
                        return new(true, "Remove user: 1) remove all authored ideas (Success)");
                    }

                    // 2) REMOVE ALL MEMBERSHIP (!=Author)
                    List<IdeaMember> getUserMembers = await _dbContext.IdeaMembers
                        .Include(x => x.Idea)
                        .Where(x => x.UserId.Equals(userId))
                        .ToListAsync();

                    if (getUserMembers.Count > 0)
                    {
                        _dbContext.IdeaMembers.RemoveRange(getUserMembers);
                        await _dbContext.SaveChangesAsync();
                        return new(true, "Remove user: 2) remove all membership (Success)");
                    }


                    ApplicationUser getUser = await _dbContext.Users
                        .Include(x => x.Ideas)
                        .ThenInclude(x => x.Idea)
                        .Include(x => x.Tags)
                        .Include(x => x.Reactions)
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
                        IEnumerable<string> chatIds = getUser.Chats
                            .Select(x => x.Chat.Id);

                        ICollection<Chat> chats = new List<Chat>();
                        foreach (var chatId in chatIds)
                        {
                            Chat getChat = await _dbContext.Chats
                                .Include(x => x.Users)
                                .Include(x => x.Messages)
                                .FirstOrDefaultAsync(x => x.Id.Equals(chatId));

                            if (getChat != null)
                                chats.Add(getChat);
                        }
                        _dbContext.Chats.RemoveRange(chats);


                        _dbContext.Users.Remove(getUser);
                        await _dbContext.SaveChangesAsync();
                        return new(true, "Remove user: 3) other (Success)");
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
