using ApplicationCore.Entities.IdeaEntity;
using ApplicationCore.Entities.UserEntity;
using ApplicationCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Config
{

    public class UserIdeaConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasMany(x => x.Ideas).WithOne(x => x.User)
                .OnDelete(DeleteBehavior.SetNull);                        
        }
    }

    public class UserFriendshipRequestsConfiguration : IEntityTypeConfiguration<FriendshipRequest>
    {
        public void Configure(EntityTypeBuilder<FriendshipRequest> builder)
        {
            builder.HasOne(x => x.RequestToUser).WithMany(x => x.FriendRequests)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
    }

    public class UserFriendshipConfiguration : IEntityTypeConfiguration<Friendship>
    {
        public void Configure(EntityTypeBuilder<Friendship> builder)
        {
            builder.HasMany(x => x.Users).WithMany(x => x.Friends);
        }
    }
}
