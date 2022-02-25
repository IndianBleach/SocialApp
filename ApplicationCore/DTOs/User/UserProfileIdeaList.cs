using ApplicationCore.DTOs.Idea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.User
{
    public class UserProfileIdeaList
    {
        public ICollection<ProfileIdeaDto> Ideas { get; set; }
        public ICollection<PageInfoDto> Pages { get; set; }
    }
}
