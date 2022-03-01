using ApplicationCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.IdeaEntity
{
    public enum IdeaRolesToUpdate
    {
        Modder = 1,
        Member = 0
    }

    public enum IdeaMemberRoles
    {
        Author = 1,
        Modder = 2,
        Member = 3
    }

    public class IdeaMember : BaseEntity
    {
        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public Idea? Idea { get; set; }
        public IdeaMemberRoles Role { get; set; }

        public IdeaMember(string userId, IdeaMemberRoles role)
        {
            UserId = userId;
            Role = role;
        }
    }
}
