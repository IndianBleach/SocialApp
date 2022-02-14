using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.AsyncLoad
{
    public class FriendRequestDto : SmallUserDto
    {
        public string FriendGuid { get; set; }
    }

    public class FriendUserDto : SmallUserDto
    { 
        public string FriendGuid { get; set; }
    }

    public class SmallUserDto
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public string AvatarName { get; set; }
    }
}
