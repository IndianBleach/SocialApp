using ApplicationCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.UserEntity
{
    public class UserAvatar : BaseEntity
    { 
        public ICollection<ApplicationUser> Users { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }

        public UserAvatar(bool isDefault, string name)
        {
            Name = name;
            IsDefault = isDefault;
            Users = new List<ApplicationUser>();
        }
    }


    public class UserContact : BaseEntity
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string Url { get; set; }

        public UserContact(string userId, string url)
        {
            UserId = userId;
            Url = url;
        }

        public UserContact(ApplicationUser user, string url)
        {
            User = user;
            Url = url;
        }
    }
}
