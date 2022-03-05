using ApplicationCore.DTOs.AsyncLoad;
using ApplicationCore.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces.Repositories
{
    public interface IAdminRepository
    {
        //Task<OperationResultDto> CreateAdminAccountAsync();    
        Task<UserSmallDto> GetAdminAccountOrNullAsync(string userId);
        Task<OperationResultDto> RemoveIdeaByIdAsync(string ideaId, string currentUserId);
        Task<OperationResultDto> RemoveAccountByIdAsync(string userId, string currentUserId);
        Task<OperationResultDto> RemoveTopicByIdAsync(string topicId, string currentUserId);
        Task<OperationResultDto> RemoveGoalByIdAsync(string goalId, string currentUserId);
        Task<OperationResultDto> CleanTopicCommentsAsync(string currentUserId);
        Task<OperationResultDto> CleanGoalTasksAsync(string currentUserId);
    }
}
