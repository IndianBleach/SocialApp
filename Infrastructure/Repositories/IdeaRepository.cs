using ApplicationCore.DTOs;
using ApplicationCore.DTOs.AsyncLoad;
using ApplicationCore.DTOs.Create;
using ApplicationCore.DTOs.Idea;
using ApplicationCore.DTOs.Tag;
using ApplicationCore.DTOs.User;
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
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class IdeaRepository : IIdeaRepository
    {
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
            if (model.AuthorGuid == null)
                return new CreateOperationResult(false, null, "При создании идеи что-то пошло не так");

            IdeaAvatar avatar = await _dbContext.IdeaAvatars
                .FirstOrDefaultAsync(x => x.IsDefault);

            IdeaStatus status = await _dbContext.IdeaStatuses
                .FirstOrDefaultAsync(x => x.Type.Equals(IdeaStatusType.FindMembers));

            if (SafetyInputHelper.CheckAntiXSSRegex(model.Name) &&
                SafetyInputHelper.CheckAntiXSSRegex(model.Description) &&
                avatar != null && 
                status != null)
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

            return new CreateOperationResult(false, null, "При создании идеи что-то пошло не так"); ;
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
                        .ForMember("Description", opt => opt.MapFrom(x => x.Description))
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
                    .ThenInclude(x => x.User)
                    .ThenInclude(x => x.Avatar)
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
                        .ForMember("Tags", opt => opt.MapFrom(x => x.Tags.Select(x => 
                            new TagDto()
                            {
                                Guid = x.Id,
                                Name = x.Name
                            })))
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
                                x.UserId.Equals(currentUserGuid)))))
                        .ForMember("MemberRequests", opt => opt.MapFrom(x => x.Invitations
                            .Where(e => e.Type.Equals(IdeaInvitationType.Join))
                            .Take(20).Select(e => new UserSmallDto(e.UserId, e.User.UserName, e.User.Avatar.Name))))
                        .ForMember("Members", opt => opt.MapFrom(x => x.Members
                            .Where(x => !x.Role.Equals(IdeaMemberRoles.Author))
                            .Take(20).Select(e => new UserSmallDto(e.UserId, e.User.UserName, e.User.Avatar.Name))))
                        );                                                      

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

        public IdeaGoalsListDto GetIdeaGoalList(string ideaGuid, int? page)
        {
            int correctPage = page ?? 1;
            int count = ConstantsHelper.CountIdeasPerPage;

            if (!string.IsNullOrEmpty(ideaGuid))
            {
                IEnumerable<IdeaGoal> getGoals = _dbContext.IdeaGoals
                    .Include(x => x.Tasks)
                    .Include(x => x.Author)
                    .ThenInclude(x => x.Avatar)
                    .Where(x => x.IdeaId.Equals(ideaGuid));

                IEnumerable<IdeaGoal> getGoalsPerPage = getGoals
                    .OrderBy(x => x.DateCreated)
                    .Skip((correctPage - 1) * count)
                    .Take(count);

                if (getGoals != null)
                {
                    var config = new MapperConfiguration(conf => conf.CreateMap<IdeaGoal, IdeaGoalDto>()
                        .ForMember("Guid", opt => opt.MapFrom(x => x.Id))
                        .ForMember("AuthorGuid", opt => opt.MapFrom(x => x.AuthorId))
                        .ForMember("AuthorName", opt => opt.MapFrom(x => x.Author.UserName))
                        .ForMember("AuthorAvatar", opt => opt.MapFrom(x => x.Author.Avatar.Name))
                        .ForMember("DatePublished", opt => opt.MapFrom(x => IdeaHelper.NormalizeDate(x.DateCreated)))
                        .ForMember("Name", opt => opt.MapFrom(x => x.Name))
                        .ForMember("Description", opt => opt.MapFrom(x => x.Description))
                        .ForMember("TotalTaskCount", opt => opt.MapFrom(x => x.Tasks.Count))
                        .ForMember("CompleteTaskCount", opt => opt.MapFrom(x => x.Tasks
                            .Where(x => x.Type.Equals(IdeaGoalTaskType.Complete)).Count()))
                        .ForMember("WaitingTaskCount", opt => opt.MapFrom(x => x.Tasks
                            .Where(x => x.Type.Equals(IdeaGoalTaskType.Waiting)).Count())));

                    var mapper = new Mapper(config);

                    ICollection<IdeaGoalDto> dtos = mapper.Map<ICollection<IdeaGoalDto>>(getGoalsPerPage);

                    IdeaGoalsListDto resDto = new()
                    {
                        Pages = _globalService.CreatePages(getGoals.Count(), page),
                        Goals = dtos
                    };

                    return resDto;
                }
            }

            IdeaGoalsListDto res = new()
            {
                Pages = new List<PageInfoDto>(),
            };

            return res;
        }

        public Task<List<IdeaStatusDto>> GetAllIdeaStatusesAsync()
        {
            return _dbContext.IdeaStatuses
                .Select(x => new IdeaStatusDto(x.Type))
                .ToListAsync();
        }

        public async Task<string> GetIdeaDescriptionAsync(string ideaGuid)
        {
            var getTopic = await _dbContext.IdeaTopics
                .Where(x => x.IdeaId.Equals(ideaGuid))
                .OrderByDescending(x => x.IsInit)
                .OrderByDescending(x => x.DateCreated)
                .FirstOrDefaultAsync();

            if (getTopic != null)
                return getTopic.Description;

            else return "Без описания";
        }

        public async Task<IdeaMemberListDto?> GetIdeaMemberListByRoleOrNull(string ideaGuid, IdeaMemberRoles byRole, int? page)
        {
            int normalizePage = page ?? 1;

            Idea getIdea = await _dbContext.Ideas
                .Include(x => x.Members)
                .ThenInclude(x => x.User)
                .ThenInclude(x => x.Avatar)
                .FirstOrDefaultAsync(x => x.Id.Equals(ideaGuid));

            if (getIdea != null)
            {
                var res = getIdea.Members
                    .Where(x => x.Role.Equals(byRole))
                    .Skip(10 * (normalizePage - 1))
                    .Take(10)
                    .Select(x => new UserSmallDto(
                        x.UserId, x.User.UserName, x.User.Avatar.Name));

                IdeaMemberListDto dto = new()
                {
                    Pages = _globalService.CreatePages(res.Count(), normalizePage),
                    Members = res
                };

                return dto;
            }

            return null;
        }
    }
}
