using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs
{
    public enum HomeNewsType
    {
        NewIdea = 1,
        NewUser = 2,
    }

    public class HomeNewsDto
    {
        public HomeNewsType Type { get; set; }
        public string Content { get; set; }
        public string UserGuid { get; set; }
        public string UserAvatarName { get; set; }
        public string UserName { get; set; }
        public string? IdeaName { get; set; }
        public string? IdeaGuid { get; set; }
        public DateTime NewsDate { get; set; }
    }
}
