using ApplicationCore.DTOs;
using ApplicationCore.DTOs.Idea;
using ApplicationCore.DTOs.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.IdeaViewModel
{
    public class IdeaAboutViewModel
    {
        public bool IsAuthorized { get; set; }
        public IdeaDetailDto Idea { get; set; }
        public IEnumerable<IdeaSmallDto> RecommendIdeas { get; set; }
        public IdeaTopicListDto TopicList { get; set; }
        public List<TagDto> Tags { get; set; }
    }
}
