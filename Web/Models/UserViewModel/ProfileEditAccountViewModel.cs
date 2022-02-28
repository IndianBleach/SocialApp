using ApplicationCore.DTOs.Tag;
using ApplicationCore.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.UserViewModel
{
    public class ProfileEditAccountViewModel
    {
        public UserEditAccountDto? UserDetail { get; set; }
        public List<TagDto> AllTags { get; set; }
    }
}
