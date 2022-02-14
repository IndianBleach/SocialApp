using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.Idea
{
    public class HomeIdeaReactionDto
    {
        public bool IsActive { get; set; }
        public string Guid { get; set; }
        public string Value { get; set; }
        public int Count { get; set; }
    }
}
