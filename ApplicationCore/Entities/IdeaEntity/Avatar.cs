using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.IdeaEntity
{
    public class IdeaAvatar : BaseEntity
    {
        public ICollection<Idea> Ideas { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }

        public IdeaAvatar(bool isDefault, string name)
        {
            Name = name;
            IsDefault = isDefault;
            Ideas = new List<Idea>();
        }
    }
}
