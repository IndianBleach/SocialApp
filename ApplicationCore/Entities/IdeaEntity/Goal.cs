using ApplicationCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.IdeaEntity
{
    public enum IdeaGoalTaskType
    {
        Complete = 1,
        Waiting = 0,
    }

    public class IdeaGoalTask : BaseEntity
    {
        public IdeaGoal Goal { get; set; }
        public string AuthorId { get; set; }
        public ApplicationUser Author { get; set; }
        public IdeaGoalTaskType Type { get; set; }
        public string Content { get; set; }
        public DateTime DateCreated { get; set; }

        public IdeaGoalTask(string authorId, IdeaGoalTaskType type,
            string content)
        {
            AuthorId = authorId;
            Type = type;
            Content = content;
            DateCreated = DateTime.Now;
        }

        public IdeaGoalTask(IdeaGoal goal, string authorId, IdeaGoalTaskType type,
            string content)
        {
            Goal = goal;
            AuthorId = authorId;
            Type = type;
            Content = content;
            DateCreated = DateTime.Now;
        }
    }

    public class IdeaGoal : BaseEntity
    {
        public Idea Idea { get; set; }
        public string AuthorId { get; set; }
        public ApplicationUser Author { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public ICollection<IdeaGoalTask> Tasks { get; set; }
        public bool IsDefault { get; set; }
        public bool IsPrivate { get; set; }


        public IdeaGoal(string authorId, string name,
           string description, bool isDefault, bool isPrivate)
        {
            AuthorId = authorId;
            Name = name;
            Description = description;
            IsDefault = isDefault;
            IsPrivate = isPrivate;
            DateCreated = DateTime.Now;
            Tasks = new List<IdeaGoalTask>();
        }

        public IdeaGoal(Idea idea, string authorId, string name,
            string description, bool isDefault, bool isPrivate)
        {
            Idea = idea;
            AuthorId = authorId;
            Name = name;
            Description = description;
            IsDefault = isDefault;
            IsPrivate = isPrivate;
            DateCreated = DateTime.Now;
            Tasks = new List<IdeaGoalTask>();
        }
    }
}
