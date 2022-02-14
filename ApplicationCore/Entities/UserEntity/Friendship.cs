using ApplicationCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.UserEntity
{
    public enum ProfileFriendshipType
    {
        NotFriends = 0,
        Request = 1,
        Accept = 2,       
    }

    public enum FriendshipType
    { 
        Request = 0,
        Accepted = 1
    }

    public class Friendship : BaseEntity
    {
        public FriendshipType Type { get; set; }
        public DateTime DateCreated {  get; set; }
        public ICollection<ApplicationUser> Users { get; set; }

        public Friendship(FriendshipType type, ICollection<ApplicationUser> users)
        {
            Users = users;
            Type = type;
        }

        public Friendship(FriendshipType type)
        {
            Users = new List<ApplicationUser>();
            Type = type;
        }
    }
}
