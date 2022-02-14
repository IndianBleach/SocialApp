using ApplicationCore.DTOs.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.Idea
{    

    public class HomeIdeaDto
    {
        public string Guid { get; set; }
        public string Name {  get; set; }
        public string Description {  get; set; }
        public string AvatarName { get; set; }
        public string DateUpdated { get; set; }
        public int CountTopics { get; set; }
        public int CountGoals { get; set; }
        public bool IsLiked { get; set; }
        public bool IsReacted { get; set; }
        public ICollection<HomeIdeaReactionDto> Reactions { get; set; }
    }

    public class IdeaSmallDto
    { 
        public string Guid {  get; set; }
        public string AvatarName { get; set; }
    }    
}
