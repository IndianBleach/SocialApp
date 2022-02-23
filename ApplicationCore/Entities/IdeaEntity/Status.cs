using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.IdeaEntity
{
    public enum IdeaStatusType
    {
        FindMembers = 1,
        Development = 2,
        Complete = 3
    }


    public class IdeaStatus : BaseEntity
    {
        public IdeaStatusType Type { get; set; }
        //public string NormalizedName { get; set; }
        public ICollection<Idea> Ideas { get; set; }

        public IdeaStatus(IdeaStatusType type)
        {
            Type = type;
            //NormalizedName = normalizedName;
            Ideas = new List<Idea>();
        }
    }
}
