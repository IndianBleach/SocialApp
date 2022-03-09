﻿using ApplicationCore.Entities.Chat;
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

            builder.HasMany(x => x.FriendRequests).WithOne(x => x.RequestToUser)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.FriendRequestsSended).WithOne(x => x.Author)
                .OnDelete(DeleteBehavior.ClientCascade);

            //
            builder.HasMany(x => x.Chats).WithOne(x => x.User)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.ChatMessages).WithOne(x => x.Author)
                .OnDelete(DeleteBehavior.Cascade);

            //            
        }
    }

    public class IdeaMemberConfiguration : IEntityTypeConfiguration<IdeaMember>
    {
        public void Configure(EntityTypeBuilder<IdeaMember> builder)
        {
            //builder.HasOne(x => x.User)
        }
    }

    /*
    public class UserFriendshipRequestsConfiguration : IEntityTypeConfiguration<FriendshipRequest>
    {
        public void Configure(EntityTypeBuilder<FriendshipRequest> builder)
        {
            builder.HasOne(x => x.RequestToUser).WithMany(x => x.FriendRequests)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
    */

    public class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.HasMany(x => x.Users).WithOne(x => x.Chat)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Messages).WithOne(x => x.Chat)
                .OnDelete(DeleteBehavior.Cascade);

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
