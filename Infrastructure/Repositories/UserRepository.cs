using ApplicationCore.DTOs.AsyncLoad;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationContext _dbContext;
        private readonly IGlobalService<Idea> _globalService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserRepository(
            ITagService tagService,
            ApplicationContext context,
            IGlobalService<Idea> globalService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _dbContext = context;
            _globalService = globalService;
            _userManager = userManager;
            _signInManager = signInManager;
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
                    .FirstOrDefault()
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
                ?.Ideas.FirstOrDefault()
                ?.Idea?.Tags?.FirstOrDefault();

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

        public ICollection<UserDto> GetUsersPerPage(int? page, string? search, string? tag, string? country, string? city)
        {
            int correctPage = page ?? 1;            

            int count = ConstantsHelper.CountUsersPerPage;

            IQueryable<ApplicationUser> getUsers;

            // country + city + search
            if (!string.IsNullOrWhiteSpace(country) &&
                (!string.IsNullOrWhiteSpace(city)))
            {
                if (!string.IsNullOrWhiteSpace(search))
                    getUsers = _dbContext.Users
                        .Include(x => x.Avatar)
                        .Include(x => x.Tags)
                        .Where(x =>
                            x.AddressCountry.StartsWith(country) &&
                            x.AddressCity.StartsWith(city) &&
                            x.UserName.StartsWith(search))
                        .Skip((correctPage - 1) * count)
                        .Take(count);

                else getUsers = _dbContext.Users
                    .Include(x => x.Avatar)
                    .Include(x => x.Tags)
                    .Where(x =>
                        x.AddressCountry.StartsWith(country) &&
                        x.AddressCity.StartsWith(city))
                    .Skip((correctPage - 1) * count)
                    .Take(count);
            }            
            // country + search            
            else if (!string.IsNullOrWhiteSpace(country))
            {
                if (!string.IsNullOrWhiteSpace(search))
                {
                    getUsers = _dbContext.Users
                    .Include(x => x.Avatar)
                    .Include(x => x.Tags)
                    .Where(x =>
                        x.AddressCountry.StartsWith(country) && 
                        x.UserName.StartsWith(search))
                    .Skip((correctPage - 1) * count)
                    .Take(count);
                }
                else
                {
                    getUsers = _dbContext.Users
                    .Include(x => x.Avatar)
                    .Include(x => x.Tags)
                    .Where(x =>
                        x.AddressCountry.StartsWith(country))
                    .Skip((correctPage - 1) * count)
                    .Take(count);
                }
            }
            // city + search
            else if (!string.IsNullOrWhiteSpace(city))
            {
                if (!string.IsNullOrWhiteSpace(search))
                {
                    getUsers = _dbContext.Users
                        .Include(x => x.Avatar)
                        .Include(x => x.Tags)
                        .Where(x =>
                            x.AddressCity.StartsWith(city) &&
                            x.UserName.StartsWith(search))
                        .Skip((correctPage - 1) * count)
                        .Take(count);
                }
                else
                {
                    getUsers = _dbContext.Users
                        .Include(x => x.Avatar)
                        .Include(x => x.Tags)
                        .Where(x =>
                            x.AddressCity.StartsWith(city))
                        .Skip((correctPage - 1) * count)
                        .Take(count);
                }
            }            
            else if (!string.IsNullOrWhiteSpace(search))
            {
                getUsers = _dbContext.Users
                    .Include(x => x.Avatar)
                    .Include(x => x.Tags)
                    .Where(x => x.UserName.StartsWith(search))
                    .Skip((correctPage - 1) * count)
                    .Take(count);
            }
            else if (!string.IsNullOrWhiteSpace(tag))
            {
                Tag? getTag = _dbContext.Tags
                    .FirstOrDefault(x => x.Id.Equals(tag));

                if (getTag == null)
                    getTag = _dbContext.Tags.FirstOrDefault();

                getUsers = _dbContext.Users
                    .Include(x => x.Avatar)
                    .Include(x => x.Tags)
                    .Where(x => x.Tags.Contains(getTag))
                    .Skip((correctPage - 1) * count)
                    .Take(count);
            }                        
            else
            {
                getUsers = _dbContext.Users
                    .Include(x => x.Avatar)
                    .Include(x => x.Tags)
                    .Skip((correctPage - 1) * count)
                    .Take(count);
            }

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

                        _ => ProfileFriendshipType.NotFriends
                    };
                }
                else
                {
                    var req = await _dbContext.FriendshipRequests
                        .FirstOrDefaultAsync(x => x.AuthorId.Equals(currentUserGuid) &&
                            x.RequestToUserId.Equals(userProfileGuid));

                    if (req != null)
                        return ProfileFriendshipType.Request;
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

        public async Task<UserProfileIdeaList> GetUserParticipationIdeaListAsync(string userId, string currentUserId, bool onlyAuthorIdeas, int? page)
        {
            ICollection<Idea> getIdeas;
            int ideasCount = 0;
            int normalizePage = page ?? 1;

            if (onlyAuthorIdeas)
            {
                ideasCount = _dbContext.Ideas
                    .Where(x => x.Members.Any(x => x.UserId.Equals(userId) &&
                        x.Role.Equals(IdeaMemberRoles.Author)))
                    .Count();

                getIdeas = await _dbContext.Ideas
                    .Include(x => x.Status)
                    .Include(x => x.Members)
                    .Include(x => x.Invitations)
                    .Include(x => x.Avatar)
                    .Where(x => x.Members.Any(x => x.UserId.Equals(userId) && 
                        x.Role.Equals(IdeaMemberRoles.Author)))
                    .Skip((ConstantsHelper.CountIdeasPerPage * (normalizePage - 1)))
                    .Take(10)
                    .ToListAsync();
            }
            else
            {
                ideasCount = _dbContext.Ideas
                    .Where(x => x.Members.Any(x => x.UserId.Equals(userId)))
                    .Count();

                getIdeas = await _dbContext.Ideas
                    .Include(x => x.Status)
                    .Include(x => x.Members)
                    .Include(x => x.Invitations)
                    .Include(x => x.Avatar)
                    .Where(x => x.Members.Any(x => x.UserId.Equals(userId)))
                    .Skip((ConstantsHelper.CountIdeasPerPage * (normalizePage - 1)))
                    .Take(10)
                    .ToListAsync();
            }

            var config = new MapperConfiguration(conf => conf.CreateMap<Idea, ProfileIdeaDto>()
                    .ForMember("Guid", opt => opt.MapFrom(x => x.Id))
                    .ForMember("Name", opt => opt.MapFrom(x => x.Name))
                    .ForMember("Avatar", opt => opt.MapFrom(x => x.Avatar.Name))
                    .ForMember("IsLiked", opt => opt.MapFrom(x => 
                        IdeaHelper.CheckIsLiked(x.Members, x.Invitations, currentUserId)))
                    .ForMember("Status", opt => opt.MapFrom(x => 
                        new IdeaStatusDto(x.Status.Type))));

            var mapper = new Mapper(config);

            ICollection<ProfileIdeaDto> ideas = mapper.Map<ICollection<ProfileIdeaDto>>(getIdeas);

            UserProfileIdeaList res = new()
            {
                Ideas = ideas,
                Pages = _globalService.CreatePages(ideasCount, normalizePage),
            };

            return res;
        }

        private string NormazileUserAddress(string? country, string? city)
        {
            if (!string.IsNullOrEmpty(country) &&
                !string.IsNullOrEmpty(city))
                return $"{country} - {city}";
            if (!string.IsNullOrEmpty(country))
                return $"{country}";
            else if (!string.IsNullOrEmpty(city))
                return $"{city}";
            else 
                return "";
        }

        public async Task<UserAboutInfoDto> GetUserAboutInfoAsync(string userId)
        {
            if (userId != null)
            {
                ApplicationUser getUser = await _dbContext.Users
                .Include(x => x.Avatar)
                .Include(x => x.Contacts)
                .Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.Id.Equals(userId));

                if (getUser != null)
                {
                    var config = new MapperConfiguration(conf => conf.CreateMap<ApplicationUser, UserAboutInfoDto>()
                        .ForMember("UserId", opt => opt.MapFrom(x => x.Id))
                        .ForMember("Name", opt => opt.MapFrom(x => x.UserName))
                        .ForMember("Description", opt => opt.MapFrom(x => x.Description))
                        .ForMember("AvatarImageName", opt => opt.MapFrom(x => x.Avatar.Name))
                        .ForMember("Tags", opt => opt.MapFrom(x => x.Tags.Select(x => x.Name)))
                        .ForMember("Address", opt => opt.MapFrom(x => 
                            NormazileUserAddress(x.AddressCountry, x.AddressCity)))
                        .ForMember("Contact", opt => opt.MapFrom(x => 
                            new UserContactDto()
                            { 
                                Name = x.Contacts.FirstOrDefault() != null ?
                                    x.Contacts.FirstOrDefault().Name : null,
                                Url = x.Contacts.FirstOrDefault() != null ?
                                    x.Contacts.FirstOrDefault().Url : null,
                            })));

                    var mapper = new Mapper(config);

                    UserAboutInfoDto dto = mapper.Map<UserAboutInfoDto>(getUser);

                    return dto;
                }
            }
            return null;
        }

        public async Task<UserEditAccountDto> GetEditAccountUserAsync(string userId)
        {
            if (userId != null)
            {
                ApplicationUser getUser = await _dbContext.Users
                .Include(x => x.Avatar)
                .Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.Id.Equals(userId));

                if (getUser != null)
                {
                    var allTags = _dbContext.Tags.ToList();

                    var config = new MapperConfiguration(conf => conf.CreateMap<ApplicationUser, UserEditAccountDto>()
                        .ForMember("UserId", opt => opt.MapFrom(x => x.Id))
                        .ForMember("Name", opt => opt.MapFrom(x => x.UserName))
                        .ForMember("Description", opt => opt.MapFrom(x => x.Description))
                        .ForMember("AvatarImageName", opt => opt.MapFrom(x => x.Avatar.Name))
                        .ForMember("Tags", opt => opt.MapFrom(x => allTags.Select(e => new TagEditDto()
                        { 
                            Guid = e.Id,
                            Name = e.Name,
                            Selected = x.Tags.Any(r => r.Id.Equals(e.Id))
                        }))));

                    var mapper = new Mapper(config);

                    UserEditAccountDto dto = mapper.Map<UserEditAccountDto>(getUser);

                    return dto;
                }
            }
            return null;
        }

        public async Task<UserEditGeneralDto> GetEditGeneralUserAsync(string userId)
        {
            if (userId != null)
            {
                ApplicationUser getUser = await _dbContext.Users
                    .Include(x => x.Avatar)
                    .Include(x => x.Tags)
                    .Include(x => x.Contacts)
                    .FirstOrDefaultAsync(x => x.Id.Equals(userId));

                if (getUser != null)
                {
                    var allTags = _dbContext.Tags.ToList();

                    var config = new MapperConfiguration(conf => conf.CreateMap<ApplicationUser, UserEditGeneralDto>()
                        .ForMember("UserId", opt => opt.MapFrom(x => x.Id))
                        .ForMember("Name", opt => opt.MapFrom(x => x.UserName))
                        .ForMember("Description", opt => opt.MapFrom(x => x.Description))
                        .ForMember("AvatarImageName", opt => opt.MapFrom(x => x.Avatar.Name))
                        .ForMember("Tags", opt => opt.MapFrom(x => x.Tags.Select(e => e.Name)))
                        .ForMember("ContactName", opt => opt.MapFrom(x => x.Contacts.FirstOrDefault() != null ? x.Contacts.FirstOrDefault().Name : null))
                        .ForMember("ContactUrl", opt => opt.MapFrom(x => x.Contacts.FirstOrDefault() != null ? x.Contacts.FirstOrDefault().Url : null)));

                    var mapper = new Mapper(config);

                    UserEditGeneralDto dto = mapper.Map<UserEditGeneralDto>(getUser);

                    return dto;
                }
            }
            return null;
        }

        public async Task<OperationResultDto> UpdateAccountSettingsAsync(string userId, UpdateAccountSettingsDto model, IEnumerable<ClaimsIdentity> identities)
        {
            ApplicationUser getUser = await _dbContext.Users
                .Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.Id.Equals(userId));

            if (getUser != null)
            {
                if (!string.IsNullOrEmpty(model.Username) &&
                    SafetyInputHelper.CheckAntiXSSRegex(model.Username))
                {
                    var userIdentity = identities.FirstOrDefault(x => x.Name == getUser.UserName);
                    if (userIdentity != null)
                    {
                        var getClaim = userIdentity.FindFirst("UserName");
                        Claim newClaim = new("UserName", model.Username);

                        await _userManager.ReplaceClaimAsync(getUser, getClaim, newClaim);
                    }
                }

                if (!string.IsNullOrEmpty(model.NewPassword) &&
                    !string.IsNullOrEmpty(model.NewPasswordConfirm) &&
                    SafetyInputHelper.CheckAntiXSSRegex(model.NewPassword))
                {
                    bool canEdit = model.NewPassword.Equals(model.NewPasswordConfirm) ?
                        await _userManager.CheckPasswordAsync(getUser, model.OldPassword) :
                        false;

                    if (canEdit)
                        await _userManager.ChangePasswordAsync(getUser, model.OldPassword, model.NewPassword);
                }

                if (model.Tags.Count > 0)
                {
                    List<Tag> updatedTags = new();
                    foreach (var tag in model.Tags)
                    {
                        Tag getTag = await _dbContext.Tags
                            .FirstOrDefaultAsync(x => x.Id.Equals(tag));

                        if (getTag != null)
                            updatedTags.Add(getTag);

                        getUser.Tags = updatedTags;
                    }
                }

                await _dbContext.SaveChangesAsync();
                return new(true, "Op. Update User (Success)");
            }

            return new(false, "Op. Update User (Failed)");
        }

        public async Task<OperationResultDto> UpdateGeneralSettingsAsync(string userId, UpdateGeneralSettingsDto model, IEnumerable<ClaimsIdentity> identities)
        {
            ApplicationUser getUser = await _dbContext.Users
                .Include(x => x.Contacts)
                .Include(x => x.Avatar)
                .FirstOrDefaultAsync(x => x.Id.Equals(userId));

            if (getUser != null)
            {
                if (model.Avatar != null)
                    if (model.Avatar.FileName != AvatarInformation.UserDefaultAvatarName)
                    {
                        using (FileStream str = new($"wwwroot/media/userAvatars/" + model.Avatar.FileName, FileMode.Create))
                        {
                            await model.Avatar.CopyToAsync(str);
                            str.Close();
                        }

                        if ((getUser.Avatar.Name != AvatarInformation.UserDefaultAvatarName) &&
                            !getUser.Avatar.IsDefault)
                            File.Delete($"wwwroot/media/userAvatars/{getUser.Avatar.Name}");

                        getUser.Avatar = new(false, model.Avatar.FileName);

                        var userIdentity = identities.FirstOrDefault(x => x.Name == getUser.UserName);
                        if (userIdentity != null)
                        {
                            var getClaim = userIdentity.FindFirst("UserAvatarName");
                            Claim newClaim = new("UserAvatarName", getUser.Avatar.Name);

                            await _userManager.ReplaceClaimAsync(getUser, getClaim, newClaim);
                        }
                    }

                if (!string.IsNullOrWhiteSpace(model.Description) &&
                    SafetyInputHelper.CheckAntiXSSRegex(model.Description))
                    getUser.Description = model.Description;

                if (!string.IsNullOrWhiteSpace(model.AddressCity) &&
                    SafetyInputHelper.CheckAntiXSSRegex(model.AddressCity))
                    getUser.AddressCity = model.AddressCity;

                if (!string.IsNullOrWhiteSpace(model.AddressCountry) &&
                    SafetyInputHelper.CheckAntiXSSRegex(model.AddressCountry))
                    getUser.AddressCountry = model.AddressCountry;

                if (!string.IsNullOrWhiteSpace(model.ContactName) &&
                    !string.IsNullOrWhiteSpace(model.ContactUrl) &&
                    SafetyInputHelper.CheckAntiXSSRegex(model.ContactName) &&
                    SafetyInputHelper.CheckAntiXSSRegex(model.ContactUrl))
                {
                    _dbContext.UserContacts.RemoveRange(getUser.Contacts);
                    getUser.Contacts = new List<UserContact>()
                    {
                        new(model.ContactUrl, model.ContactName)
                    };
                }

                IdentityResult res = await _userManager.UpdateAsync(getUser);                

                if (res.Succeeded)
                {
                    await _signInManager.SignInAsync(getUser, false);
                    return new(true, "Op. Update User (Success)");
                }                
            }

            return new(false, "Op. Update User (Failed)");
        }

        public IEnumerable<IdeaUserParticipationDto> GetUserParticipations(string userId)
        {
            var getRoles = _dbContext.IdeaMembers
                .Include(x => x.Idea)
                .Where(x => x.UserId.Equals(userId))                
                .Take(20);

            var config = new MapperConfiguration(conf => conf.CreateMap<IdeaMember, IdeaUserParticipationDto>()
                .ForMember("Guid", opt => opt.MapFrom(x => x.Idea.Id))
                .ForMember("Name", opt => opt.MapFrom(x => x.Idea.Name))
                .ForMember("RoleName", opt => opt.MapFrom(x => 
                    IdeaHelper.NormalizeRoleName(x.Role))));                

            var mapper = new Mapper(config);

            IEnumerable<IdeaUserParticipationDto> dtos = mapper.Map<IEnumerable<IdeaUserParticipationDto>>(getRoles);

            return dtos;
        }
    }
}
