using ApplicationCore.DTOs;
using ApplicationCore.DTOs.Idea;
using ApplicationCore.DTOs.Tag;
using System.Collections.Generic;

namespace WebUi.Models
{
    public class HomeIdeasViewModel
    {
        public ICollection<PageInfoDto> Pages { get; set; }
        public ICollection<HomeNewsDto> LastNews { get; set; }
        public ICollection<IdeaSmallDto> RecommendIdeas { get; set; }
        public ICollection<HomeIdeaDto> Ideas { get; set; }
        public ICollection<SearchReactionDto> SearchReactions { get; set; }
        public List<TagDto> Tags { get; set; }
        public bool IsAuthorized{ get; set; }
        public string? SortReact = null;
        public string? Search = null;
        public string? SearchTag = null;
        public string? SearchKey = null;
    }
}
