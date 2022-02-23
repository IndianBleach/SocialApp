using ApplicationCore.DTOs.AsyncLoad.Idea;
using ApplicationCore.DTOs.Idea;
using ApplicationCore.DTOs.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.IdeaViewModel
{
    public class IdeaEditGeneralViewModel
    {
        public IdeaDetailDto Idea { get; set; }
        public IEnumerable<IdeaSmallDto> RecommendIdeas { get; set; }
        public List<TagDto> Tags { get; set; }
        public IEnumerable<EditTagDto> EditTags { get; set; }
        public List<IdeaStatusDto> EditStatuses { get; set; }
        public string EditDescription { get; set; }
    }
}
