using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.Idea
{
    public class ProfileIdeaDto
    {
        public IdeaStatusDto Status {get; set;}
        public string Guid { get; set; }
        public string Avatar { get; set; }
        public string Name { get; set; }
        public bool IsLiked { get; set; }
    }   
}
