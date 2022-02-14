using ApplicationCore.Entities;
using ApplicationCore.Entities.IdeaEntity;
using ApplicationCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
        public ICollection<Idea> Ideas { get; set; }

        public Tag(string name)
        {
            Name = name;
            Users = new List<ApplicationUser>();
            Ideas = new List<Idea>();
        }
    }
}
