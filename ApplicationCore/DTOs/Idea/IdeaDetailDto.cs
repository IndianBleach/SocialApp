using ApplicationCore.DTOs.Tag;
using ApplicationCore.DTOs.User;
using ApplicationCore.Entities.IdeaEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.Idea
{
    public class IdeaModderDto
    { 
        public string UserGuid { get; set; }
        public string UserAvatar { get; set; }

        public IdeaModderDto(string userGuid, string userAvatar)
        {
            UserAvatar = userAvatar;
            UserGuid = userGuid; 
        }
    }

    public class SimilarIdeaDto
    {
        public string IdeaGuid { get; set; }
        public string IdeaAvatar { get; set; }
    }

    public class IdeaStatusDto
    { 
        public int TypeValue { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string EmojiValue { get; set; }

        public IdeaStatusDto(IdeaStatusType baseType)
        {
            switch (baseType)
            {
                case IdeaStatusType.Complete:
                    TypeValue = (int)baseType;
                    Name = "Выполнена";
                    Description = "Ждет улучшений и предложений";
                    EmojiValue = "🌌";
                    break;

                case IdeaStatusType.FindMembers:
                    TypeValue = (int)baseType;
                    Name = "Поиск участников";
                    Description = "Нужны новые люди в команду";
                    EmojiValue = "🧐";
                    break;

                case IdeaStatusType.Development:
                    TypeValue = (int)baseType;
                    Name = "В разработке";
                    Description = "Активно развивается";
                    EmojiValue = "🚀";
                    break;
            }
        }
    }

    public class IdeaDetailDto
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public string AvatarName { get; set; }
        public IdeaStatusDto Status { get; set; }
        public IEnumerable<TagDto> Tags { get; set; }
        public IEnumerable<IdeaModderDto> Modders { get; set; }
        public ICollection<HomeIdeaReactionDtoUpdated> Reactions { get; set; }
        public ICollection<IdeaDetailReactionCount> AllReactionsByType { get; set; }
        public ICollection<UserSmallDto> Members { get; set; }
        public ICollection<UserSmallDto> MemberRequests { get; set; }
        public CurrentUserRoleDto CurrentRole { get; set; }
        public bool IsReacted { get; set; }        
        public bool IsLiked { get; set; }
        public bool IsSecret { get; set; }
        public bool CanUserWathing { get; set; }
    }
}
