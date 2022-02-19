using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.Idea
{
    public class IdeaSmallDto
    {
        public string Guid { get; set; }
        public string AvatarName { get; set; }

        public IdeaSmallDto(string guid, string avatar)
        {
            Guid = guid;
            AvatarName = avatar;
        }

        public IdeaSmallDto()
        {
        }
    }
}
