using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.User
{
    public class UserSmallDto
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }

        public UserSmallDto(string guid, string name, string avatar)
        {
            Avatar = avatar;
            Guid = guid;
            Name = name;
        }
    }


}
