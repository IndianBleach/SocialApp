using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.Idea
{
    public class IdeaTopicListDto
    {
        public IEnumerable<IdeaTopicDto> Topics {get;set;}
        public IEnumerable<PageInfoDto> Pages { get; set; }
    }

    public class IdeaTopicDto
    {
        public bool ByModder { get; set; }
        public string Guid { get; set; }
        public string AuthorGuid { get; set; }
        public string AuthorName { get; set; }
        public string AuthorAvatar { get; set; }
        public string DatePublished { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CommentsCount { get; set; }
        //public bool CanEdit { get; set; }        
    }
}
