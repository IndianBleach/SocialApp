using ApplicationCore.DTOs.Idea;
using ApplicationCore.Entities.IdeaEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public static class IdeaHelper
    {
        public static bool CheckIsLiked(
            ICollection<IdeaMember> members,
            ICollection<IdeaInvitation> requests,
            string? currentUserId)
        {
            if (!string.IsNullOrEmpty(currentUserId))
            {
                bool isLiked = members.Any(x => x.UserId.Equals(currentUserId));
                if (!isLiked)
                    isLiked = requests.Any(x => x.UserId.Equals(currentUserId));
                return isLiked;
            }
            else return false;
        }


        public static List<HomeIdeaReactionDto> GroupIdeaReactions(
            ICollection<Reaction> baseReactionsTypes,
            ICollection<IdeaReaction> reactions, 
            string? currentUserGuid)
        {
            var allTypes = reactions.Select(x => x.Reaction.Id);

            var allReactsByType = allTypes.Select(react => reactions.Where(x => x.Reaction.Id
                    .Equals(react)));

            List<HomeIdeaReactionDto> dtos = allReactsByType.Select(react => new HomeIdeaReactionDto()
            {
                Count = react.Count(),
                Guid = react.First().Reaction.Id,
                Value = react.First().Reaction.Name,
                IsActive = react.FirstOrDefault(x => x.UserId.Equals(currentUserGuid)) != null
            })
            .ToList();

            int needFill = 4 - dtos.Count();
            for (int i = 0; i < needFill; i++)
            {
                Reaction? getReact = baseReactionsTypes.ElementAtOrDefault(i);
                if (getReact != null)
                {
                    dtos.Add(new()
                    {
                        Count = 0,
                        Guid = getReact.Id,
                        Value = getReact.Name
                    });
                }
            }

            return dtos.ToList();
        }
    }
}
