using ApplicationCore.Entities;
using ApplicationCore.Entities.Chat;
using ApplicationCore.Entities.IdeaEntity;
using ApplicationCore.Entities.UserEntity;
using ApplicationCore.Interfaces.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Identity
{
    public class ApplicationUser : IdentityUser, IBaseEntity
    {
        public DateTime DateCreated { get; set; }
        public string? Description { get; set; }
        public ICollection<Friendship> Friends { get; set; }
        public ICollection<FriendshipRequest> FriendRequests { get; set; }
        public ICollection<IdeaMember> Ideas { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public ICollection<IdeaReaction> Reactions {  get; set; }
        public ICollection<IdeaInvitation> Invitations {  get; set; }
        public ICollection<IdeaRepost> Reposts { get; set; }
        public ICollection<ChatUser> Chats { get; set; }
        public UserContact? UserContact { get; set; }
        public UserAvatar Avatar { get; set; }
        public DateTime CheckEventsDate { get; set; }

        public ApplicationUser()
        {
            DateCreated = DateTime.Now;
            Friends = new List<Friendship>();
            Ideas = new List<IdeaMember>();
            Tags = new List<Tag>();
            Reactions = new List<IdeaReaction>();
            Invitations = new List<IdeaInvitation>();
            Reposts = new List<IdeaRepost>();
            Chats = new List<ChatUser>();
            CheckEventsDate = DateTime.Now;
        }
    }
}
