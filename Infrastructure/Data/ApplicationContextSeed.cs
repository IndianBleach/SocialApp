using ApplicationCore.Entities;
using ApplicationCore.Entities.IdeaEntity;
using ApplicationCore.Entities.UserEntity;
using Infrastructure.Constants;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class ApplicationContextSeed
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole("admin"));
            await roleManager.CreateAsync(new IdentityRole("user"));
        }

        public static async Task SeedDatabaseAsync(ApplicationContext dbContext)
        {
            if (dbContext.Tags.Any()) return;

            dbContext.Tags.AddRange(new Tag[]
            {
                new Tag("Спорт"),
                new Tag("Фильмы"),
                new Tag("Экология"),
                new Tag("Космос"),
                new Tag("Электроника"),
                new Tag("Игры"),
                new Tag("Дизайн"),
                new Tag("Другое"),
                new Tag("ИИ"),
                new Tag("Интернет"),
                new Tag("Здоровье"),
                new Tag("Юмор"),
            });

            dbContext.IdeaStatuses.AddRange(new IdeaStatus[]
            {
                new IdeaStatus(IdeaStatusType.Complete),
                new IdeaStatus(IdeaStatusType.FindMembers),
                new IdeaStatus(IdeaStatusType.Development)
            });

            dbContext.Reactions.AddRange(new Reaction[]
            {
                new("😍", null),
                new("😴", null),
                new("👏", null),
                new("😼", null),
            });           

            dbContext.IdeaAvatars.Add(new IdeaAvatar(true, AvatarInformation.IdeaDefaultAvatarName));

            dbContext.UserAvatars.Add(new UserAvatar(true, AvatarInformation.UserDefaultAvatarName));

            await dbContext.SaveChangesAsync();
        }
    }
}
