using ApplicationCore.DTOs;
using ApplicationCore.DTOs.Create;
using ApplicationCore.DTOs.Idea;
using ApplicationCore.Entities;
using ApplicationCore.Entities.IdeaEntity;
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
    public class IdeaRepository : IIdeaRepository
    {
        //private readonly UserManager<ApplicationUser> _userManager;
        //private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationContext _dbContext;
        private readonly ITagService _tagService;
        private readonly IGlobalService<Idea> _globalService;

        public IdeaRepository(
            ITagService tagService,
            ApplicationContext context,
            IGlobalService<Idea> globalService)
        {
            _dbContext = context;
            _tagService = tagService;
            _globalService = globalService;
        }

        public async Task<CreateOperationResult> CreateIdeaAsync(CreateIdeaDto model)
        {
            IdeaAvatar avatar = await _dbContext.IdeaAvatars
                .FirstOrDefaultAsync(x => x.IsDefault);

            IdeaStatus status = await _dbContext.IdeaStatuses
                .FirstOrDefaultAsync(x => x.Type.Equals(IdeaStatusType.FindMembers));

            if (avatar != null && status != null)
            {
                ICollection<Tag> tags = await _tagService.CreateTagListAsync(model.Tags);

                List<IdeaMember> members = new()
                {
                    new(model.AuthorGuid, IdeaMemberRoles.Author)
                };

                List<IdeaTopic> topics = new()
                {
                    new(model.AuthorGuid, "Об этой идее", model.Description, true, false, true)
                };

                Idea createIdea = new(model.Name, model.IsPrivate,
                    avatar, status, tags, members, topics);

                await _dbContext.Ideas.AddAsync(createIdea);

                await _dbContext.SaveChangesAsync();

                return new CreateOperationResult(true, createIdea.Id, "Идея успешно создана!");
            }

            return new CreateOperationResult(false, null, "Что-то пошло не так"); ;
        }        

        public ICollection<HomeIdeaDto> GetIdeasPerPage(int? page, string? currentUserId, string? sortReact, string? key, string? tag, string? search)
        {
            int correctPage = page ?? 1;

            int count = ConstantsHelper.CountIdeasPerPage;

            ICollection<Reaction> allBaseReacts = _dbContext.Reactions.ToList();

            IQueryable<Idea> getIdeas;
            if (!string.IsNullOrEmpty(sortReact))
            {
                getIdeas = _dbContext.Ideas
                .Include(x => x.Reactions)
                    .ThenInclude(x => x.Reaction)
                .Include(x => x.Avatar)
                .Include(x => x.Topics)
                .Include(x => x.Goals)
                .Include(x => x.Invitations)
                .Include(x => x.Members)
                .OrderByDescending(x => x.Reactions
                    .Where(x => x.ReactionId.Equals(sortReact)).Count())
                .Skip((correctPage - 1) * count)
                .Take(count);
            }
            else if (!string.IsNullOrWhiteSpace(search))
            {
                getIdeas = _dbContext.Ideas
                   .Include(x => x.Reactions)
                       .ThenInclude(x => x.Reaction)
                   .Include(x => x.Avatar)
                   .Include(x => x.Topics)
                   .Include(x => x.Goals)
                   .Include(x => x.Invitations)
                   .Include(x => x.Members)
                   .Where(x => x.Name.StartsWith(search))
                   .Skip((correctPage - 1) * count)
                   .Take(count);
            }
            else if (!string.IsNullOrEmpty(tag))
            {
                Tag? getTag = _dbContext.Tags
                    .FirstOrDefault(x => x.Id.Equals(tag));

                if (getTag == null)
                    getTag = _dbContext.Tags.First();

                getIdeas = _dbContext.Ideas
                   .Include(x => x.Reactions)
                       .ThenInclude(x => x.Reaction)
                   .Include(x => x.Avatar)
                   .Include(x => x.Topics)
                   .Include(x => x.Goals)
                   .Include(x => x.Invitations)
                   .Include(x => x.Members)
                   .Where(x => x.Tags.Contains(getTag))
                   .Skip((correctPage - 1) * count)
                   .Take(count);
            }
            else if (!string.IsNullOrEmpty(key))
            {
                string keyName = key == "popular" ? "popular" :
                    key == "new" ? "new" : "new";

                if (keyName == "popular")
                {
                    getIdeas = _dbContext.Ideas
                       .Include(x => x.Reactions)
                           .ThenInclude(x => x.Reaction)
                       .Include(x => x.Avatar)
                       .Include(x => x.Topics)
                       .Include(x => x.Goals)
                       .Include(x => x.Invitations)
                       .Include(x => x.Members)
                       .OrderByDescending(x => x.Reactions.Count)
                       .Skip((correctPage - 1) * count)
                       .Take(count);
                }
                else
                {                   
                    getIdeas = _dbContext.Ideas
                       .Include(x => x.Reactions)
                           .ThenInclude(x => x.Reaction)
                       .Include(x => x.Avatar)
                       .Include(x => x.Topics)
                       .Include(x => x.Goals)
                       .Include(x => x.Invitations)
                       .Include(x => x.Members)
                       .OrderByDescending(x => x.DateCreated)
                       .Skip((correctPage - 1) * count)
                       .Take(count);
                }

            }
            else
            {
                getIdeas = _dbContext.Ideas
                   .Include(x => x.Reactions)
                       .ThenInclude(x => x.Reaction)
                   .Include(x => x.Avatar)
                   .Include(x => x.Topics)
                   .Include(x => x.Goals)
                   .Include(x => x.Invitations)
                   .Include(x => x.Members)
                   .Skip((correctPage - 1) * count)
                   .Take(count);
            }

            var config = new MapperConfiguration(conf => conf.CreateMap<Idea, HomeIdeaDto>()
                .ForMember("DateUpdated", opt => opt.MapFrom(x => IdeaHelper.NormalizeDate(x.DateUpdated)))
                .ForMember("Guid", opt => opt.MapFrom(x => x.Id))
                .ForMember("Name", opt => opt.MapFrom(x => x.Name))
                .ForMember("Description", opt => opt.MapFrom(x => x.Topics
                    .First().Description))
                .ForMember("AvatarName", opt => opt.MapFrom(x => x.Avatar.Name))
                .ForMember("Reactions", opt => opt.MapFrom(x => IdeaHelper.GroupIdeaReactions(allBaseReacts, x.Reactions, currentUserId)))
                .ForMember("CountGoals", opt => opt.MapFrom(x => x.Goals.Count))
                .ForMember("CountTopics", opt => opt.MapFrom(x => x.Topics.Count))
                .ForMember("IsLiked", opt => opt.MapFrom(x => 
                    IdeaHelper.CheckIsLiked(x.Members, x.Invitations, currentUserId))));

            var mapper = new Mapper(config);
            
            ICollection<HomeIdeaDto> dtos = mapper.Map<ICollection<HomeIdeaDto>>(getIdeas);

            foreach (var item in dtos)
                item.IsReacted = item.Reactions.Any(x => x.IsActive);

            return dtos;
        }

        public async Task<ICollection<HomeIdeaReactionDto>> GroupAllReactionsAsync()
        {
            var dtos = await _dbContext.Reactions
                .Select(x => new HomeIdeaReactionDto()
                {
                    Count = x.Ideas.Count,
                    Guid = x.Id,
                    Value = x.Name
                }).ToListAsync();

            return dtos;
        }

        public ICollection<IdeaSmallDto> GetRecommendIdeas(string? userGuid)
        {
            if (userGuid != null)
            {
                var test = _dbContext.Users;

                Tag? getUserFirstTag = _dbContext.Users?
                    .Include(x => x.Tags)
                    .FirstOrDefault(x => x.Id == userGuid)
                    ?.Tags.FirstOrDefault();

                if (getUserFirstTag != null)
                {
                    IQueryable<Idea>? getIdeas = _dbContext.Ideas
                    .Where(x => x.Tags.Contains(getUserFirstTag))?
                    .Take(5);

                    if (getIdeas != null)
                    {
                        var config = new MapperConfiguration(conf => conf.CreateMap<Idea, IdeaSmallDto>()
                        .ForMember("Guid", opt => opt.MapFrom(x => x.Id))
                        .ForMember("AvatarName", opt => opt.MapFrom(x => x.Avatar.Name)));

                        var mapper = new Mapper(config);

                        ICollection<IdeaSmallDto> dtos = mapper.Map<List<IdeaSmallDto>>(getIdeas);

                        return dtos;
                    }
                }
            }

            return new List<IdeaSmallDto>();
        }

        public IdeaTopicListDto GetIdeaTopicList(string ideaGuid, int? page)
        {
            int correctPage = page ?? 1;
            int count = ConstantsHelper.CountIdeasPerPage;

            if (!string.IsNullOrEmpty(ideaGuid))
            {
                IEnumerable<IdeaTopic> getTopics = _dbContext.IdeaTopics
                    .Include(x => x.Comments)
                    .Include(x => x.Author)
                    .ThenInclude(x => x.Avatar)
                    .Where(x => x.IdeaId.Equals(ideaGuid));

                IEnumerable<IdeaTopic> getTopicsPerPage = getTopics
                    .OrderByDescending(x => x.IsDefault)
                    .OrderByDescending(x => x.DateCreated)
                    .Skip((correctPage - 1) * count)
                    .Take(count);                

                if (getTopics != null)
                {
                    var config = new MapperConfiguration(conf => conf.CreateMap<IdeaTopic, IdeaTopicDto>()
                        .ForMember("ByModder", opt => opt.MapFrom(x => x.IsDefault))
                        .ForMember("Guid", opt => opt.MapFrom(x => x.Id))
                        .ForMember("AuthorGuid", opt => opt.MapFrom(x => x.AuthorId))
                        .ForMember("AuthorName", opt => opt.MapFrom(x => x.Author.UserName))
                        .ForMember("AuthorAvatar", opt => opt.MapFrom(x => x.Author.Avatar.Name))
                        .ForMember("DatePublished", opt => opt.MapFrom(x => IdeaHelper.NormalizeDate(x.DateCreated)))
                        .ForMember("Name", opt => opt.MapFrom(x => x.Name))
                        .ForMember("Name", opt => opt.MapFrom(x => x.Description))
                        .ForMember("CommentsCount", opt => opt.MapFrom(x => x.Comments.Count)));                        

                    var mapper = new Mapper(config);

                    ICollection<IdeaTopicDto> dtos = mapper.Map<ICollection<IdeaTopicDto>>(getTopicsPerPage);

                    IdeaTopicListDto resDto = new()
                    {
                        Pages = _globalService.CreatePages(getTopics.Count(), page),
                        Topics = dtos
                    };

                    return resDto;
                }                
            }

            IdeaTopicListDto res = new()
            {
                Pages = new List<PageInfoDto>(),
                Topics = new List<IdeaTopicDto>()
            };

            return res;
        }

        public async Task<IdeaDetailDto?> GetIdeaDetailOrNullAsync(string currentUserGuid, string ideaGuid)
        {
            if (!string.IsNullOrEmpty(currentUserGuid) &&
                !string.IsNullOrEmpty(ideaGuid))
            {
                Idea? getIdea = await _dbContext.Ideas
                    .Include(x => x.Invitations)
                    .Include(x => x.Status)
                    .Include(x => x.Avatar)
                    .Include(x => x.Members)
                    .ThenInclude(x => x.User)
                    .ThenInclude(x => x.Avatar)
                    .Include(x => x.Tags)
                    .Include(x => x.Reactions)
                    .FirstOrDefaultAsync(x => x.Id.Equals(ideaGuid));

                if (getIdea != null)
                {
                    var reacts = await _dbContext.Reactions.ToListAsync();

                    var getCurUser = await _dbContext.Users
                        .FirstOrDefaultAsync(x => x.Id.Equals(currentUserGuid));

                    var config = new MapperConfiguration(conf => conf.CreateMap<Idea, IdeaDetailDto>()
                        .ForMember("Guid", opt => opt.MapFrom(x => x.Id))
                        .ForMember("Name", opt => opt.MapFrom(x => x.Name))
                        .ForMember("AvatarName", opt => opt.MapFrom(x => x.Avatar.Name))
                        .ForMember("Status", opt => opt.MapFrom(x => new IdeaStatusDto(x.Status.Type)))
                        .ForMember("Tags", opt => opt.MapFrom(x => x.Tags.Select(x => x.Name)))
                        .ForMember("IsSecret", opt => opt.MapFrom(x => x.IsPrivate))
                        .ForMember("Modders", opt => opt.MapFrom(x => x.Members
                            .Where(x => x.Role < IdeaMemberRoles.Member)
                            .Select(u => new IdeaModderDto(u.UserId, u.User.Avatar.Name))))
                        .ForMember("Reactions", opt => opt.MapFrom(x =>
                            IdeaHelper.GroupIdeaReactions(reacts, x.Reactions, currentUserGuid)))
                        .ForMember("IsLiked", opt => opt.MapFrom(x => 
                            IdeaHelper.CheckIsLiked(x.Members, x.Invitations, currentUserGuid)))
                        .ForMember("CurrentRole", opt => opt.MapFrom(x => 
                            new CurrentUserRoleDto(x.Members.FirstOrDefault(x => 
                                x.UserId.Equals(currentUserGuid))))));                                                      

                    var mapper = new Mapper(config);

                    IdeaDetailDto dto = mapper.Map<IdeaDetailDto>(getIdea);

                    // Current User Can Wathing
                    if ((dto.IsSecret) && (dto.CurrentRole.Role == CurrentUserRoleTypes.Viewer))
                        dto.CanUserWathing = false;
                    else dto.CanUserWathing = true;

                    // Already Reacted
                    dto.IsReacted = dto.Reactions.Any(x => x.IsActive);

                    return dto;
                }
            }
            return null;
        }

        public async Task<IEnumerable<IdeaSmallDto>> GetSimilarOrTrendsIdeasAsync(string? ideaGuid)
        {
            IQueryable<Idea> similar;
            if (!string.IsNullOrEmpty(ideaGuid))
            {
                Idea getIdea = await _dbContext.Ideas
                    .Include(x => x.Tags)
                    .FirstOrDefaultAsync(x => x.Id.Equals(ideaGuid));
                
                if (getIdea != null)
                    similar = _dbContext.Ideas
                        .Include(x => x.Tags)
                        .Where(x => x.Tags
                        .Contains(getIdea.Tags.First()))
                        .Take(5);
            }
            similar = _dbContext.Ideas
                .Include(x => x.Reactions)
                .OrderBy(x => x.Reactions.Count)
                .Take(5);

            return similar.Select(x => new IdeaSmallDto(x.Id, x.Avatar.Name));
        }
    }
}
