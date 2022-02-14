using ApplicationCore.DTOs.Idea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.User
{
    public class UserDetailDto
    {
        public string UserId {  get; set;}
        public string Name { get; set; }
        public string Description { get; set; }
        public string AvatarImageName { get; set; }
        public ICollection<string> Tags { get; set; }
    }
}
