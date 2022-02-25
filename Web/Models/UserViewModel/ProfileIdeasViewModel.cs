﻿using ApplicationCore.DTOs.Tag;
using ApplicationCore.DTOs.User;
using ApplicationCore.Entities.UserEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.UserViewModel
{
    public class ProfileIdeasViewModel
    {
        public UserDetailDto? UserDetail { get; set; }
        public List<TagDto> AllTags { get; set; }
        public UserProfileIdeaList IdeaList { get; set; }
        public ProfileFriendshipType FriendType { get; set; }
        public bool IsAuthorized { get; set; }        
        public bool IsSelfProfile { get; set; } 
        public string? Section { get; set; }
        public bool AuthoredIdeas { get; set; }
    }
}
