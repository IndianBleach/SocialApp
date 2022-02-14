using ApplicationCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.UserEntity
{
    public class FriendshipRequest : BaseEntity
    {
        public string AuthorId { get; set; }
        public ApplicationUser Author {get;set;}
        public string RequestToUserId { get; set; }
        public ApplicationUser RequestToUser { get; set; }

        public FriendshipRequest(string authorId, string requestToUserId)
        {
            AuthorId = authorId;
            RequestToUserId = requestToUserId;
        }

        public FriendshipRequest(ApplicationUser author, ApplicationUser requestToUser)
        {
            Author = author;
            AuthorId = author.Id;
            RequestToUser = requestToUser;
            RequestToUserId = requestToUser.Id;
        }
    }
}
