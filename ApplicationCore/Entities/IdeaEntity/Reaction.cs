using ApplicationCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.IdeaEntity
{
    public class Reaction : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<IdeaReaction> Ideas { get; set; }

        public Reaction(string name, string? description)
        {
            Name = name;
            Description = description;
            Ideas = new List<IdeaReaction>();
        }
    }

    public class IdeaReaction : BaseEntity
    {
        public string IdeaId { get; set; }
        public Idea Idea { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string ReactionId { get; set; }
        public Reaction Reaction { get; set; }

        public IdeaReaction(ApplicationUser user, Idea idea, Reaction reaction)
        {
            User = user;
            Idea = idea;
            Reaction = reaction;
        }

        public IdeaReaction(string userId, string reactionId, string ideaId)
        {
            IdeaId = ideaId;
            UserId = userId;
            ReactionId = reactionId;
        }
    }
}
