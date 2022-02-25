using ApplicationCore.Entities.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.IdeaEntity
{
    public class Idea : BaseEntity
    {
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public ICollection<IdeaMember> Members { get; set; }
        public ICollection<IdeaReaction> Reactions { get; set; }
        public int AvatarId { get; set; }
        public IdeaAvatar Avatar { get; set; }
        public ICollection<ChatMessage> Reposts { get; set; }
        public ICollection<IdeaInvitation> Invitations { get; set; }
        public ICollection<IdeaTopic> Topics { get; set; }
        public ICollection<IdeaGoal> Goals { get; set; }
        public IdeaContact? Contact { get; set; }
        public int StatusId { get; set; }
        public IdeaStatus Status { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated {  get; set; }

        public Idea()
        {
            Members = new List<IdeaMember>();
            Reactions = new List<IdeaReaction>();
            Reposts = new List<ChatMessage>();
            Invitations = new List<IdeaInvitation>();
            Topics = new List<IdeaTopic>();
            Goals = new List<IdeaGoal>();
            DateCreated = DateTime.Now;
            DateUpdated = DateTime.Now;
        }

        public Idea(
            string name, bool isPrivate, IdeaAvatar avatar,
             IdeaStatus status, ICollection<Tag> tags, ICollection<IdeaMember> members,
            ICollection<IdeaTopic> topics)
        {
            Name = name;
            IsPrivate = isPrivate;
            Tags = tags;
            Avatar = avatar;
            Status = status;
            Contact = null;
            Members = members;
            Reactions = new List<IdeaReaction>();
            Reposts = new List<ChatMessage>();
            Invitations = new List<IdeaInvitation>();
            Topics = topics;
            Goals = new List<IdeaGoal>();
            DateCreated = DateTime.Now;
            DateUpdated = DateTime.Now;
        }
    }
}
