using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.User
{
    public class UserList
    {
        public ICollection<UserDto> Users { get; set; }
        public ICollection<PageInfoDto> Pages { get; set; }

        public UserList(
            ICollection<UserDto> users,
            ICollection<PageInfoDto> pages)
        {
            Pages = pages;
            Users = users;
        }
    }
}
