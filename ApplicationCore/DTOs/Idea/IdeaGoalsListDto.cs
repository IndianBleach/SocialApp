using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.Idea
{
    public class IdeaGoalsListDto
    {
        public IEnumerable<IdeaGoalDto> Goals { get; set; }
        public IEnumerable<PageInfoDto> Pages { get; set; }
    }

    public class IdeaGoalDto
    {
        public string Guid { get; set; }
        public string AuthorGuid { get; set; }
        public string AuthorName { get; set; }
        public string AuthorAvatar { get; set; }
        public string DatePublished { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CompleteTaskCount { get; set; }
        public string WaitingTaskCount { get; set; }
        public string TotalTaskCount { get; set; }
    }
}
