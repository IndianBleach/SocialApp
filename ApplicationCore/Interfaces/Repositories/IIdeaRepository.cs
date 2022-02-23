using ApplicationCore.DTOs;
using ApplicationCore.DTOs.Create;
using ApplicationCore.DTOs.Idea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IIdeaRepository
    {
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