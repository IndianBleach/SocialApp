using ApplicationCore.DTOs.Idea;
using ApplicationCore.DTOs.Tag;
using ApplicationCore.DTOs.User;
using ApplicationCore.Entities;
using ApplicationCore.Entities.Chat;
using ApplicationCore.Entities.IdeaEntity;
using ApplicationCore.Entities.UserEntity;
using ApplicationCore.Identity;
using ApplicationCore.Interfaces;
using AutoMapper;
using Infrastructure.Constants;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationContext _dbContext;
        private readonly ITagService _tagService;

        public UserRepository(ITagService tagService, ApplicationContext context)
        {
            _dbContext = context;
            _tagService = tagService;
        }        

        public async Task<ICollection<ChatUserDto>> GetUserChatsAsync(string userGuid)
        {
            var exc = _dbContext.Chats
                .Include(x => x.Messages)
                .Include(x => x.Users)
                .ThenInclude(x => x.User)
                .ThenInclude(x => x.Avatar)
                .Where(x => x.Users
                    .Any(y => y.UserId == userGuid));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Chat, ChatUserDto>()
                .ForMember("UserGuid", opt => opt.MapFrom(x => x.Users
                    .FirstOrDefault(e => e.User.Id != userGuid).User.Id))
                .ForMember("ChatGuid", opt => opt.MapFrom(x => x.Id))
                .ForMember("UserName", opt => opt.MapFrom(x => x.Users
                    .Where(x => x.User.Id != userGuid).Single().User.UserName))
                .ForMember("UserAvatar", opt => opt.MapFrom(x => x.Users
                    .Where(x => x.UserId != userGuid).First().User.Avatar.Name))
                .ForMember("LastMessageDate", opt => opt.MapFrom(x => GeneratePublishDate(x.Messages
                    .OrderBy(x => x.DateCreated)
                    .First()
                    .DateCreated)));
            });

            var mapper = new Mapper(config);

            var resp = mapper.Map<IQueryable<Chat>, ICollection<ChatUserDto>>(exc);

            return resp;
        }

        private static string GeneratePublishDate(DateTime dateCreated)
        {
            var res = DateTime.Now - dateCreated;

            int years = 0;
            int months = 0;
            for (int i = 1; i <= res.Days; i++)
            {
                if (i > 30) months++;
                if (i > 364) years++;
            }

            if (years > 0) return $"{years} год. назад";
            else if (months > 0) return $"{months} мес. назад";
            else if (res.Days < 1) return "cегодня";
            else return $"{res.Days} дн. назад";
        }

        public async Task<ICollection<RecommendUserDto>?> GetRecommendsUsersOrNullAsync(string? userGuid)
        {
            if (userGuid != null)
            {
                Tag? getFirstTag = _dbContext.Users
                .Include(x => x.Ideas)
                    .ThenInclude(x => x.Idea)
                    .ThenInclude(x => x.Tags)
                .FirstOrDefault(x => x.Id.Equals(userGuid))
                .Ideas.FirstOrDefault()
                ?.Idea.Tags.FirstOrDefault();

                if (getFirstTag != null)
                {
                    var res = _dbContext.Users
                        .Include(x => x.Tags)
                        .Where(x => x.Tags.Contains(getFirstTag))
                        .Take(5)
                        .Select(x => new RecommendUserDto()
                        {
                            AvatarImageName = x.Avatar.Name,
                            DateJoined = GeneratePublishDate(x.DateCreated),
                            Guid = x.Id,
                            Name = x.UserName
                        });

                    return await res.ToListAsync();
                }
            }

            return null;
        }

        private string? GenerateAddressOrNull(string? country, string? city)
        {
            if ((country == null) && (city == null))
                return null;

            if (string.IsNullOrEmpty(country) && string.IsNullOrEmpty(city))
                return null;

            if (!string.IsNullOrEmpty(country) && !string.IsNullOrEmpty(city))
                return $"({country}, {city})";

            if (!string.IsNullOrEmpty(city))
                return $"({city})";

            if (!string.IsNullOrEmpty(country))
                return $"({country})";

            return null;
        }

        public ICollection<UserDto> GetUsersPerPage(int? page)
        {
            int correctPage = page ?? 1;

            int count = ConstantsHelper.CountUsersPerPage;

            IQueryable<ApplicationUser> getUsers = _dbContext.Users
                .Include(x => x.Avatar)
                .Include(x => x.Tags)
                .Skip((correctPage - 1) * count)
                .Take(count);

            var config = new MapperConfiguration(conf => conf.CreateMap<ApplicationUser, UserDto>()
                .ForMember("Guid", opt => opt.MapFrom(x => x.Id))
                .ForMember("Address", opt => opt.MapFrom(x => GenerateAddressOrNull(x.AddressCountry, x.AddressCity)))
                .ForMember("Name", opt => opt.MapFrom(x => x.UserName))
                .ForMember("Description", opt => opt.MapFrom(x => x.Description))
                .ForMember("AvatarImageName", opt => opt.MapFrom(x => x.Avatar.Name))
                .ForMember("Tags", opt => opt.MapFrom(x => x.Tags.Select(x => new TagDto()
                { 
                    Guid = x.Id,
                    Name = x.Name
                }))));                

            var mapper = new Mapper(config);

            ICollection<UserDto> dtos = mapper.Map<ICollection<UserDto>>(getUsers);

            return dtos;
        }

        public async Task<UserDetailDto?> GetUserDetailOrNullAsync(string userGuid)
        {
            if (userGuid != null)
            {
                ApplicationUser getUser = await _dbContext.Users
                .Include(x => x.Avatar)
                .Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.Id.Equals(userGuid));

                if (getUser != null)
                {
                    var config = new MapperConfiguration(conf => conf.CreateMap<ApplicationUser, UserDetailDto>()
                    .ForMember("UserId", opt => opt.MapFrom(x => x.Id))
                    .ForMember("Name", opt => opt.MapFrom(x => x.UserName))
                    .ForMember("Description", opt => opt.MapFrom(x => x.Description))
                    .ForMember("AvatarImageName", opt => opt.MapFrom(x => x.Avatar.Name))
                    .ForMember("Tags", opt => opt.MapFrom(x => x.Tags.Select(x => x.Name))));

                    var mapper = new Mapper(config);

                    UserDetailDto dto = mapper.Map<UserDetailDto>(getUser);

                    return dto;
                }
            }
            return null;
        }

        public ICollection<HomeIdeaDto> GetUserIdeasPerPage(int? page, string userGuid)
        {
            if (userGuid != null)
            {
                int correctPage = page ?? 1;

                int count = Constants.ConstantsHelper.CountIdeasPerPage;

                ICollection<Reaction> allBaseReacts = _dbContext.Reactions.ToList();

                var getIdeas = _dbContext.Ideas
                    .Include(x => x.Members)
                    .ThenInclude(x => x.User)
                    .Include(x => x.Reactions)
                        .ThenInclude(x => x.Reaction)
                    .Include(x => x.Avatar)
                    .Include(x => x.Topics)
                    .Include(x => x.Goals)
                    .Where(x => x.Members.Any(x => x.UserId.Equals(userGuid) && x.Role.Equals(IdeaMemberRoles.Author)))
                    .Skip((correctPage - 1) * count)
                    .Take(count);

                if (getIdeas != null)
                {
                    var config = new MapperConfiguration(conf => conf.CreateMap<Idea, HomeIdeaDto>()
                    .ForMember("Guid", opt => opt.MapFrom(x => x.Id))
                    .ForMember("Name", opt => opt.MapFrom(x => x.Name))
                    .ForMember("Description", opt => opt.MapFrom(x => x.Topics
                        .First().Description))
                    .ForMember("AvatarName", opt => opt.MapFrom(x => x.Avatar.Name))
                    .ForMember("Reactions", opt => opt.MapFrom(x => IdeaHelper.GroupIdeaReactions(allBaseReacts, x.Reactions, userGuid)))
                    .ForMember("CountGoals", opt => opt.MapFrom(x => x.Goals.Count))
                    .ForMember("CountTopics", opt => opt.MapFrom(x => x.Topics.Count)));

                    var mapper = new Mapper(config);

                    ICollection<HomeIdeaDto> dtos = mapper.Map<ICollection<HomeIdeaDto>>(getIdeas);

                    return dtos;
                }
            }

            return new List<HomeIdeaDto>();
        }

        public int GetUserIdeasCountByRole(string userGuid, IdeaMemberRoles role)
        {
            int getCount = _dbContext.Ideas
                .Include(x => x.Members)
                .ThenInclude(x => x.User)
                .Include(x => x.Reactions)
                    .ThenInclude(x => x.Reaction)
                .Where(x => x.Members.Any(x => x.UserId.Equals(userGuid) && x.Role.Equals(role)))
                .Count();

            return getCount;
        }

        public async Task<ProfileFriendshipType> CheckFriendsAsync(string currentUserGuid, string userProfileGuid)
        {
            ApplicationUser getFirst = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Id.Equals(currentUserGuid));

            ApplicationUser getSec = await _dbContext.Users
               .FirstOrDefaultAsync(x => x.Id.Equals(userProfileGuid));

            if ((getFirst != null) && (getSec != null))
            {
                var res = await _dbContext.Friendships
                .FirstOrDefaultAsync(x => x.Users.Contains(getFirst) && x.Users.Contains(getSec));

                if (res != null)
                {
                    return res.Type switch
                    {
                        FriendshipType.Accepted
                            => ProfileFriendshipType.Accept,

                        FriendshipType.Request
                            => ProfileFriendshipType.Request,

                        _ => ProfileFriendshipType.NotFriends
                    };
                }
            }

            return ProfileFriendshipType.NotFriends;
        }

        public bool CheckIsSelfProfile(string currentUserGuid, string userProfileGuid)
            => currentUserGuid.Equals(userProfileGuid);

        public async Task<ActiveChatUserDto?> GetChatUserOrNullAsync(string? userGuid, string currentUserGuid)
        {
            if ((userGuid != null) && (currentUserGuid != userGuid) &&
                (currentUserGuid != null))
            {
                ApplicationUser getUser = await _dbContext.Users
                    .Include(x => x.Avatar)
                    .FirstOrDefaultAsync(x => x.Id.Equals(userGuid));

                Chat? getChat = await _dbContext.Chats
                    .FirstOrDefaultAsync(x => x.Users.All(x =>
                        x.UserId.Equals(getUser.Id) || x.UserId.Equals(currentUserGuid)));

                if (getUser != null)
                {
                    return new()
                    {
                        UserAvatar = getUser.Avatar.Name,
                        UserGuid = getUser.Id,
                        UserName = getUser.UserName,
                        ChatGuid = getChat?.Id
                    };                
                }
            }

            return null;
        }
    }
}
