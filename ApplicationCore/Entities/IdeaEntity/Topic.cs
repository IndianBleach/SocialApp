using ApplicationCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.IdeaEntity
{
    public class IdeaTopicComment : BaseEntity
    {
        public string TopicId { get; set; }
        public IdeaTopic Topic { get; set; }
        public string AuthorId { get; set; }
        public ApplicationUser Author { get; set; }
        public string Message { get; set; }
        public DateTime DateCreated { get; set; }

        public IdeaTopicComment(string topicId, string authorId, string message)
        {
            AuthorId = authorId;
            Message = message;
            TopicId = topicId;
            DateCreated = DateTime.Now;
        }

        public IdeaTopicComment(IdeaTopic topic, string authorId, string message)
        {
            AuthorId = authorId;
            Message = message;
            Topic = topic;
            DateCreated = DateTime.Now;
        }
    }

    public class IdeaTopic : BaseEntity
    {
        public string IdeaId { get; set; }
        public Idea Idea { get; set; }
        public string AuthorId { get; set; }
        public ApplicationUser Author { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public ICollection<IdeaTopicComment> Comments { get; set; }
        public bool IsDefault { get; set; }
        public bool OnlyForModders { get; set; }
        public bool IsInit { get; set; }

        public IdeaTopic(
            string authorId, string name,
            string description, bool isDefault, bool onlyForModders, bool isInit)
        {
            AuthorId = authorId;
            Name = name;
            Description = description;
            IsDefault = isDefault;
            OnlyForModders = onlyForModders;
            Comments = new List<IdeaTopicComment>();
            DateCreated = DateTime.Now;
        }

        public IdeaTopic(
            Idea idea, string authorId, string name,
            string description, bool isDefault, bool onlyForModders)
        {
            Idea = idea;
            AuthorId = authorId;
            Name = name;
            Description = description;
            IsDefault = isDefault;
            OnlyForModders = onlyForModders;
            Comments = new List<IdeaTopicComment>();
            DateCreated = DateTime.Now;
        }
    }
}
