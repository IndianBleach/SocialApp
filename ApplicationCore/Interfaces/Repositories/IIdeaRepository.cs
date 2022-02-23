using ApplicationCore.DTOs;
using ApplicationCore.DTOs.Create;
using ApplicationCore.DTOs.Idea;
using ApplicationCore.DTOs.User;
using ApplicationCore.Entities.IdeaEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IIdeaRepository
    {
        Task<IdeaMemberListDto?> GetIdeaMemberListByRoleOrNull(string ideaGuid, IdeaMemberRoles byRole, int? page);
        Task<string> GetIdeaDescriptionAsync(string ideaGuid);
        Task<List<IdeaStatusDto>> GetAllIdeaStatusesAsync();
        IdeaGoalsListDto GetIdeaGoalList(string ideaGuid, int? page);
        Task<IEnumerable<IdeaSmallDto>> GetSimilarOrTrendsIdeasAsync(string? ideaGuid);
        IdeaTopicListDto GetIdeaTopicList(string ideaGuid, int? page);
        Task<IdeaDetailDto?> GetIdeaDetailOrNullAsync(string currentUserGuid, string ideaGuid);
        Task<CreateOperationResult> CreateIdeaAsync(CreateIdeaDto model);
        Task<ICollection<HomeIdeaReactionDto>> GroupAllReactionsAsync();
        ICollection<IdeaSmallDto> GetRecommendIdeas(string? userGuid);
        ICollection<HomeIdeaDto> GetIdeasPerPage(int? page, string? currentUserId, string? sortReact, string? key, string? tag, string? search);
    }
}