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
        Task<CreateOperationResult> CreateIdeaAsync(CreateIdeaDto model);
        Task<ICollection<HomeIdeaReactionDto>> GroupAllReactionsAsync();
        ICollection<IdeaSmallDto> GetRecommendIdeas(string? userGuid);
        ICollection<HomeIdeaDto> GetIdeasPerPage(int? page, string? currentUserId);
    }
}