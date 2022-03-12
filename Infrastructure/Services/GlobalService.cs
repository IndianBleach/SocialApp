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
            => CreatePages(_dbContext.Set<T>().Count(), currentPage);

        public ICollection<PageInfoDto> CreatePages(int totalCount, int? currentPage)
        {
            List<PageInfoDto> pageInfoDtos = new();

            int countPages = (totalCount / ConstantsHelper.OrderByCount) + 1;

            if (totalCount % ConstantsHelper.OrderByCount == 0) countPages--;

            if (countPages <= 1) pageInfoDtos.Add(new(true, currentPage ?? 1));
            else if (currentPage == countPages)
            {
                if (countPages > 2)
                {
                    pageInfoDtos.Add(new(false, (currentPage ?? 1) - 2));
                    pageInfoDtos.Add(new(false, (currentPage ?? 1) - 1));
                    pageInfoDtos.Add(new(true, currentPage ?? 1));
                }
                else
                {
                    pageInfoDtos.Add(new(false, (currentPage ?? 1) - 1));
                    pageInfoDtos.Add(new(true, currentPage ?? 1));
                }
            }
            else
            {
                if (countPages > 2)
                {
                    if (currentPage == 1)
                    {
                        pageInfoDtos.Add(new(true, currentPage ?? 1));
                        pageInfoDtos.Add(new(false, (currentPage ?? 1) + 1));
                        pageInfoDtos.Add(new(false, (currentPage ?? 1) + 2));
                    }
                    else
                    {
                        pageInfoDtos.Add(new(false, (currentPage ?? 1) - 1));
                        pageInfoDtos.Add(new(true, currentPage ?? 1));
                        pageInfoDtos.Add(new(false, (currentPage ?? 1) + 1));
                    }
                }
                else
                {
                    pageInfoDtos.Add(new(true, currentPage ?? 1));
                    pageInfoDtos.Add(new(false, (currentPage ?? 1) + 1));
                }
            }

            return pageInfoDtos;
        }

        public string? CreateSearchResultString(string? tag, string? search, string? country, string? city)
        {
            // поиск по рез. Username с тегом (, страна - , город - )
            if (!string.IsNullOrWhiteSpace(search) &&
                !string.IsNullOrWhiteSpace(tag))
                return $"Поиск по рез. {search} по тегу";

            else if (!string.IsNullOrWhiteSpace(country) &&
                !string.IsNullOrWhiteSpace(city))
                return search != null ? $"Поиск по рез. {search}, страна - {country}, город - {city}" :
                    $"Поиск по рез. страна - {country}, город - {city}";

            else if (!string.IsNullOrWhiteSpace(country))
                return search != null ? $"Поиск по рез. {search}, страна - {country}" :
                    $"Поиск по рез. страна - {country}";

            else if (!string.IsNullOrWhiteSpace(city))
                return search != null ? $"Поиск по рез. {search}, страна - {city}" :
                   $"Поиск по рез. город - {city}";

            if (!string.IsNullOrWhiteSpace(tag))
                return "Поиск по тегу";

            if (!string.IsNullOrWhiteSpace(search))
                return $"Поиск по рез. {search}";

            return null;
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

            return users.OrderByDescending(x => x.NewsDate)
                .ToList();
        }

        public async Task<ICollection<SearchReactionDto>> GetSearchReactionsAsync()
            => await _dbContext.Reactions
                .Select(x => new SearchReactionDto()
                {
                    Guid = x.Id,
                    Value = x.Name
                })
                .ToListAsync();
    }
}
