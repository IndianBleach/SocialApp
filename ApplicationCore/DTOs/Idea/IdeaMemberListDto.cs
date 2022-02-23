using ApplicationCore.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.Idea
{
    public class IdeaMemberListDto
    {
        public IEnumerable<UserSmallDto> Members { get; set; }
        public ICollection<PageInfoDto> Pages { get; set; }
    }
}
