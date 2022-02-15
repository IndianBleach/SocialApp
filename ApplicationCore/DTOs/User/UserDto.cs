using ApplicationCore.DTOs.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.User
{
    public class UserDto
    {
        public string Guid { get; set; }
        public string Name {  get; set; }
        public string Description { get; set; }
        public string AvatarImageName { get; set; }
        public ICollection<TagDto> Tags { get; set; }  
        public string? Address { get; set; }
    }

    public class RecommendUserDto
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public string DateJoined { get; set; }
        public string AvatarImageName { get; set; }
    }

}
