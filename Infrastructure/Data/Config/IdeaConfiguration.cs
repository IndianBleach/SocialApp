using ApplicationCore.Entities.IdeaEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Config
{
    public class IdeaTopicCommentConfiguration : IEntityTypeConfiguration<IdeaTopicComment>
    {
        public void Configure(EntityTypeBuilder<IdeaTopicComment> builder)
        {
            builder.HasOne(x => x.Topic).WithMany(x => x.Comments)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
    }

    public class IdeaGoalTaskConfiguration : IEntityTypeConfiguration<IdeaGoalTask>
    {
        public void Configure(EntityTypeBuilder<IdeaGoalTask> builder)
        {
            builder.HasOne(x => x.Goal).WithMany(x => x.Tasks)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
    }

    public class IdeaContactConfiguration : IEntityTypeConfiguration<IdeaContact>
    {
        public void Configure(EntityTypeBuilder<IdeaContact> builder)
        {
            builder.HasOne(x => x.Idea).WithOne(x => x.Contact)
                .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}
