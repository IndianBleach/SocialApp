using ApplicationCore.DTOs;
using ApplicationCore.DTOs.Tag;
using ApplicationCore.DTOs.User;
using System.Collections.Generic;

namespace WebUi.Models
{
    public class FindUsersViewModel
    {
        public ICollection<PageInfoDto> Pages { get; set; }
        public ICollection<HomeNewsDto> LastNews { get; set; }
        public ICollection<RecommendUserDto>? RecommendUsers { get; set; }
        public ICollection<UserDto> Users { get; set; }
        public List<TagDto> Tags { get; set; }
        public bool IsAuthorized { get; set; }
    }
}
