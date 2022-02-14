using ApplicationCore.DTOs;
using ApplicationCore.DTOs.Idea;
using ApplicationCore.DTOs.Tag;
using ApplicationCore.DTOs.User;
using ApplicationCore.Entities.UserEntity;
using System.Collections.Generic;

namespace WebUi.Models.UserViewModel
{
    public class ProfileIndexViewModel
    {
        public UserDetailDto? User { get; set; }
        public List<TagDto> Tags { get; set; }
        public bool IsAuthorized { get; set; }
        public ProfileFriendshipType FriendType { get; set; }
        public bool IsSelfProfile { get; set; }
        public ICollection<HomeIdeaDto> Ideas { get; set; }
        public ICollection<PageInfoDto> Pages { get; set; }
    }
}
