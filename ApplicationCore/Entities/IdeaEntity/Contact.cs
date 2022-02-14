using ApplicationCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.IdeaEntity
{
    public class IdeaContact : BaseEntity
    {
        public string IdeaId { get; set; }
        public Idea Idea { get; set; }
        public string Url { get; set; }

        public IdeaContact(string url, string ideaId)
        {
            Url = url;
            IdeaId = ideaId;
        }

        public IdeaContact(Idea idea, string url)
        {
            Idea = idea;
            Url = url;
        }
    }
}
