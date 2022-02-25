using ApplicationCore.Entities;
using ApplicationCore.Entities.Chat;
using ApplicationCore.Entities.IdeaEntity;
using ApplicationCore.Entities.UserEntity;
using ApplicationCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Data
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Idea> Ideas { get; set; }
        public DbSet<IdeaAvatar> IdeaAvatars { get; set; }
        public DbSet<IdeaContact> IdeaContacts { get; set; }
        public DbSet<IdeaGoal> IdeaGoals { get; set; }
        public DbSet<IdeaGoalTask> IdeaGoalTasks { get; set; }
        public DbSet<IdeaInvitation> IdeaInvitations { get; set; }
        public DbSet<IdeaMember> IdeaMembers { get; set; }
        public DbSet<IdeaReaction> IdeaReactions { get; set; }
        //public DbSet<IdeaRepost> IdeaReposts { get; set; }
        public DbSet<IdeaStatus> IdeaStatuses { get; set; }
        public DbSet<IdeaTopic> IdeaTopics { get; set; }
        public DbSet<IdeaTopicComment> IdeaTopicComments { get; set; }
        public DbSet<Tag> Tags { get; set; }        
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<UserContact> UserContacts { get; set; }

        public DbSet<FriendshipRequest> FriendshipRequests { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatUser> ChatUsers { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        public DbSet<UserAvatar> UserAvatars { get; set; }
        public DbSet<Reaction> Reactions { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}