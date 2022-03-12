using ApplicationCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.IdeaEntity
{
    public enum IdeaInvitationType
    {
        Join = 1,
        Invite = 2,
    }

    public class IdeaInvitation : BaseEntity
    {
        public IdeaInvitationType Type { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        //public string IdeaId { get; set; }
        public Idea Idea { get; set; }
        public DateTime DateCreated { get; set; }

        public IdeaInvitation(IdeaInvitationType type, string userId)
        {
            UserId = userId;
            Type = type;
            DateCreated = DateTime.Now;
        }

        public IdeaInvitation(IdeaInvitationType type, string userId, Idea idea)
        {
            //IdeaId = idea.Id;
            UserId = userId;
            Type = type;
            Idea = idea;
            DateCreated = DateTime.Now;
        }
    }
}
