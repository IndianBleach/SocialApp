using ApplicationCore.DTOs;
using ApplicationCore.DTOs.Idea;
using ApplicationCore.Entities;
using ApplicationCore.Entities.IdeaEntity;
using ApplicationCore.Interfaces;
using ApplicationCore.Interfaces.Entities;
using Infrastructure.Constants;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class GlobalService<T> : IGlobalService<T> where T : class, IBaseEntity
    {
        private readonly ApplicationContext _dbContext;

        public GlobalService(ApplicationContext context)
        {
            _dbContext = context;
        }

        public ICollection<PageInfoDto> CreatePages(int? currentPage)
        {
            List<PageInfoDto> pageInfoDtos = new List<PageInfoDto>();

            int orderByCount = ConstantsHelper.OrderByCount;

            int totalCount = _dbContext.Set<T>().Count();

            return CreatePages(totalCount, currentPage);
        }

        public ICollection<PageInfoDto> CreatePages(int totalCount, int? currentPage)
        {
            List<PageInfoDto> pageInfoDtos = new List<PageInfoDto>();

            int orderByCount = ConstantsHelper.OrderByCount;

            int countPages = (totalCount / orderByCount) + 1;

            int correctPage = currentPage ?? 1;

            if (totalCount % orderByCount == 0) countPages--;

            if (countPages <= 1) pageInfoDtos.Add(new(true, correctPage));
            else if (currentPage == countPages)
            {
                if (countPages > 2)
                {
                    pageInfoDtos.Add(new(false, correctPage - 2));
                    pageInfoDtos.Add(new(false, correctPage - 1));
                    pageInfoDtos.Add(new(true, correctPage));
                }
                else
                {
                    pageInfoDtos.Add(new(false, correctPage - 1));
                    pageInfoDtos.Add(new(true, correctPage));
                }
            }
            else
            {
                if (countPages > 2)
                {
                    if (currentPage == 1)
                    {
                        pageInfoDtos.Add(new(true, correctPage));
                        pageInfoDtos.Add(new(false, correctPage + 1));
                        pageInfoDtos.Add(new(false, correctPage + 2));
                    }
                    else
                    {
                        pageInfoDtos.Add(new(false, correctPage - 1));
                        pageInfoDtos.Add(new(true, correctPage));
                        pageInfoDtos.Add(new(false, correctPage + 1));
                    }
                }
                else
                {
                    pageInfoDtos.Add(new(true, correctPage));
                    pageInfoDtos.Add(new(false, correctPage + 1));
                }
            }

            return pageInfoDtos;
        }

        public async Task<ICollection<HomeNewsDto>> GetLastNewsAsync()
        {
            List<HomeNewsDto> users = await _dbContext.Users
                .Include(x => x.Avatar)
                .OrderByDescending(x => x.DateCreated)
                .Take(3)
                .Select(x => new HomeNewsDto()
                { 
                    Content = "Новый пользователь",
                    IdeaGuid = null,
                    IdeaName = null,
                    Type = HomeNewsType.NewUser,
                    UserAvatarName = x.Avatar.Name,
                    UserName = x.UserName,
                    NewsDate = x.DateCreated,
                    UserGuid = x.Id
                })
                .ToListAsync();

            List<HomeNewsDto> ideas = await _dbContext.Ideas
                .Include(x => x.Avatar)
                .Include(x => x.Members)
                .ThenInclude(x => x.User)
                .ThenInclude(x => x.Avatar)
                .OrderByDescending(x => x.DateCreated)
                .Take(2)
                .Select(x => new HomeNewsDto()
                {
                    Content = "Новая идея",
                    IdeaGuid = x.Id,
                    IdeaName = x.Name,
                    Type = HomeNewsType.NewIdea,
                    UserAvatarName = x.Members
                    .FirstOrDefault(x => x.Role.Equals(IdeaMemberRoles.Author))
                        .User.Avatar.Name,
                    UserName = x.Members
                    .FirstOrDefault(x => x.Role.Equals(IdeaMemberRoles.Author))
                        .User.UserName,
                    NewsDate = x.DateCreated,
                    UserGuid = x.Members
                    .FirstOrDefault(x => x.Role.Equals(IdeaMemberRoles.Author))
                        .User.Id
                })
                .ToListAsync();

            users.AddRange(ideas);

            List<HomeNewsDto> dtos = users.OrderByDescending(x => x.NewsDate)
                .ToList();

            return dtos;
        }

        public async Task<ICollection<SearchReactionDto>> GetSearchReactionsAsync()
        {
            var reacts = await _dbContext.Reactions
                .Select(x => new SearchReactionDto()
                {
                    Guid = x.Id,
                    Value = x.Name
                })
                .ToListAsync();

            return reacts;
        }
    }
}
