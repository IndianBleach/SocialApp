using ApplicationCore.DTOs.Tag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.User
{
    public class TagEditDto
    { 
        public string Guid { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
    }

    public class UserEditAccountDto
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AvatarImageName { get; set; }
        public ICollection<TagEditDto> Tags { get; set; }
    }


    public class UserEditGeneralDto
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AvatarImageName { get; set; }
        public ICollection<string> Tags { get; set; }
        public string? AddressCountry { get; set; }
        public string? AddressCity { get; set; }
        public string? ContactUrl { get; set; }
        public string? ContactName { get; set; }
    }
}
