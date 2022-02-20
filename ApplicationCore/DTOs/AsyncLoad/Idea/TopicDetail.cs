using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.AsyncLoad.Idea
{
    public class TopicDetailCommentDto
    { 
        public string AuthorGuid { get; set; }
        public string AuthorName { get; set; }
        public string AuthorAvatar { get; set; }
        public string Comment { get; set; }
        public string DatePublished { get; set; }
        public string Guid { get; set; }
    }

    public class TopicDetail
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DatePublished { get; set; }
        public string AuthorGuid { get; set; }
        public string AuthorAvatar { get; set; }
        public bool CanEdit { get; set; }
        public bool ByModder { get; set; }
        public ICollection<TopicDetailCommentDto> Comments { get; set; }
    }
}
