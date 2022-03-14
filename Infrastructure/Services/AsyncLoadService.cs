using ApplicationCore.ChatHub;
using ApplicationCore.DTOs.AsyncLoad;
using ApplicationCore.DTOs.AsyncLoad.Chat;
using ApplicationCore.DTOs.AsyncLoad.Idea;
using ApplicationCore.Entities;
using ApplicationCore.Entities.Chat;
using ApplicationCore.Entities.IdeaEntity;
using ApplicationCore.Entities.UserEntity;
using ApplicationCore.Identity;
using ApplicationCore.Interfaces;
using AutoMapper;
using Infrastructure.Constants;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AsyncLoadService : IAsyncLoadService
    {
        private readonly ApplicationContext _dbContext;
        private IHubContext<ChatHub> _chatContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public AsyncLoadService(
            ApplicationContext context,
            IHubContext<ChatHub> chatContext,
            UserManager<ApplicationUser> userManager)
        {
            _dbContext = context;
            _chatContext = chatContext;
            _userManager = userManager;
        }

        //fix
        public async Task<OperationResultDto> SendFriendRequestAsync(string userGuid, string friendGuid)
        {
            var getUser = await _dbContext.Users
                .Include(x => x.Friends)
                .FirstOrDefaultAsync(x => x.Id.Equals(userGuid));

            var getUserSec = await _dbContext.Users
                .Include(x => x.Friends)
                .FirstOrDefaultAsync(x => x.Id.Equals(friendGuid));

            var getAlreadyReq = await _dbContext.FriendshipRequests
                .FirstOrDefaultAsync(x => x.AuthorId.Equals(friendGuid) &&
                    x.RequestToUserId.Equals(userGuid));

            if ((getAlreadyReq != null) && (getUser != null) && (getUserSec != null))
            {
                Friendship createFriendship = new(FriendshipType.Accepted, new List<ApplicationUser>()
                {
                    getUser,
                    getUserSec
                });

                await _dbContext.Friendships.AddAsync(createFriendship);
                _dbContext.FriendshipRequests.Remove(getAlreadyReq);
                await _dbContext.SaveChangesAsync();
                return new OperationResultDto(true, "Запрос отправлен");
            }
            if ((getUser != null) && (getUserSec != null))
            {
                FriendshipRequest createReq = new(getUser, getUserSec);

                await _dbContext.FriendshipRequests.AddAsync(createReq);
                await _dbContext.SaveChangesAsync();
                return new OperationResultDto(true, "Запрос отправлен");
            }

            return new OperationResultDto(false, "Что-то пошло не так");
        }

        public async Task<IEnumerable<RepostToUserDto>> LoadRepostUsersAsync(string userGuid)
        {
            var users = await LoadUserFriendsAsync(userGuid);

            return users.Select(x => new RepostToUserDto()
            {
                AvatarName = x.AvatarName,
                Guid = x.Guid,
                Name = x.Name
            });
        }
        
        public async Task<IEnumerable<FriendUserDto>> LoadUserFriendsAsync(string userGuid)
        {            
            var getFriends = await _dbContext.Friendships
                .Include(x => x.Users)
                .ThenInclude(x => x.Avatar)
                .Where(x => x.Users.Any(x => x.Id.Equals(userGuid)))  
                .ToListAsync();

            List<FriendUserDto> dtos = new List<FriendUserDto>();

            foreach (var friend in getFriends)
            { 
                var dtoFriend = friend.Users.FirstOrDefault(x => x.Id != userGuid);

                if (dtoFriend != null)
                {
                    dtos.Add(new()
                    {
                        FriendGuid = friend.Id,
                        AvatarName = dtoFriend.Avatar.Name,
                        Guid = dtoFriend.Id,
                        Name = dtoFriend.UserName
                    });
                }
            }

            return dtos;          
        }

        //fix
        public async Task<OperationResultDto> AcceptFriendRequestAsync(string guid)
        {
            var getRequest = await _dbContext.FriendshipRequests
                .FirstOrDefaultAsync(x => x.Id.Equals(guid));

            if (getRequest != null)
            {
                List<ApplicationUser> users = new List<ApplicationUser>();

                ApplicationUser getFirst = await _dbContext.Users
                    .FirstOrDefaultAsync(x => x.Id.Equals(getRequest.AuthorId));

                ApplicationUser getSecond = await _dbContext.Users
                    .FirstOrDefaultAsync(x => x.Id.Equals(getRequest.RequestToUserId));                

                if ((getFirst != null) && (getSecond != null))
                {
                    users.Add(getSecond);
                    users.Add(getFirst);

                    _dbContext.FriendshipRequests.Remove(getRequest);

                    var createFriend = new Friendship(FriendshipType.Accepted, users);
                    await _dbContext.Friendships.AddAsync(createFriend);
                    await _dbContext.SaveChangesAsync();
                    return new OperationResultDto(true, "Запрос выполнен");
                }

                await _dbContext.SaveChangesAsync();
                return new(true, "Операция прошла успешно");                
            }
            return new(false, "Что-то пошло не так");
        }

        public async Task<OperationResultDto> RemoveFriendAsync(string guid, string userGuid)
        {
            var getUser = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Id.Equals(userGuid));

            if (getUser != null)
            {
                var getFriendship = await _dbContext.Friendships
                .Include(x => x.Users)
                .FirstOrDefaultAsync(x => x.Id.Equals(guid) &&
                x.Users.Contains(getUser));

                if (getFriendship != null)
                {
                    _dbContext.Friendships.Remove(getFriendship);
                    await _dbContext.SaveChangesAsync();
                    return new(true, "Операция прошла успешно");
                }
            }
            return new(false, "Что-то пошло не так");
        }

        //fix
        public async Task<IEnumerable<FriendRequestDto>> LoadUserFriendRequestsAsync(string userGuid)
        {
            var getFriends = await _dbContext.FriendshipRequests
                .Include(x => x.Author)
                .ThenInclude(x => x.Avatar)
                .Where(x => x.RequestToUserId.Equals(userGuid))
                .ToListAsync();            

            var config = new MapperConfiguration(conf => conf.CreateMap<FriendshipRequest, FriendRequestDto>()
                .ForMember("Guid", opt => opt.MapFrom(x => x.Id))
                .ForMember("AvatarName", opt => opt.MapFrom(x => x.Author.Avatar.Name))
                .ForMember("FriendGuid", opt => opt.MapFrom(x => x.Id))
                .ForMember("Name", opt => opt.MapFrom(x => x.Author.UserName)));

            var mapper = new Mapper(config);

            IEnumerable<FriendRequestDto> dtos = mapper.Map<IEnumerable<FriendRequestDto>>(getFriends);

            return dtos;
        }

        public async Task<IEnumerable<SmallUserDto>> LoadNewChatUsersAsync(string guid)
        {
            var getFriends = await _dbContext.Friendships
                .Include(x => x.Users)
                .ThenInclude(x => x.Avatar)
                .Where(x => x.Users.Any(x => x.Id.Equals(guid)))
                .ToListAsync();

            var getUser = await _dbContext.Users
                .Include(x => x.Chats)
                .ThenInclude(x => x.Chat)
                .ThenInclude(x => x.Users)
                .FirstOrDefaultAsync(x => x.Id.Equals(guid));

            var activeChatsUserId = getUser.Chats
                .Select(x => x.Chat.Users.FirstOrDefault(x => !x.UserId.Equals(guid)));

            List<SmallUserDto> dtos = new List<SmallUserDto>();

            foreach (var friend in getFriends)
            {
                var dtoFriend = friend.Users.FirstOrDefault(x => x.Id != guid);

                if (!activeChatsUserId.Any(x => x.UserId == dtoFriend.Id))
                {
                    if (dtoFriend != null)
                    {
                        dtos.Add(new()
                        {
                            AvatarName = dtoFriend.Avatar.Name,
                            Guid = dtoFriend.Id,
                            Name = dtoFriend.UserName
                        });
                    }
                }                
            }

            return dtos;
        }

        private async Task<ChatMessage> CreateChatAsync(string authorGuid, string userSecGuid, string message)
        {
            if (SafetyInputHelper.CheckAntiXSSRegex(message))
            {
                ApplicationUser? messageAuthor = await _dbContext.Users
                    .Include(x => x.Avatar)
                    .FirstOrDefaultAsync(x => x.Id.Equals(authorGuid));

                ApplicationUser? toUser = await _dbContext.Users
                    .Include(x => x.Avatar)
                    .FirstOrDefaultAsync(x => x.Id.Equals(userSecGuid));

                bool isExist = _dbContext.Chats
                    .Include(x => x.Users)
                    .Any(x => x.Users.All(x => x.Id == authorGuid ||
                        x.Id == userSecGuid));

                if (!isExist)
                {
                    Chat createChat = new Chat();
                    ChatMessage createMessage = new ChatMessage(messageAuthor, message, createChat);
                    ChatUser chatUser = new ChatUser(authorGuid, createChat);
                    ChatUser chatUserSec = new ChatUser(userSecGuid, createChat);

                    createChat.Messages.Add(createMessage);
                    createChat.Users.Add(chatUser);
                    createChat.Users.Add(chatUserSec);

                    createMessage.Chat = createChat;

                    await _dbContext.Chats.AddAsync(createChat);
                    await _dbContext.SaveChangesAsync();

                    return createMessage;
                }
            }

            return null;
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


        private void FillRepeatMessages(ICollection<ChatMessageDto> messages)
        {
            if (messages.Count > 1)
            {
                for (int i = 1; i < messages.Count; i++)
                {
                    messages.ElementAt(i).IsRepeat = messages.ElementAt(i).AuthorGuid
                        .Equals(messages.ElementAt(i - 1).AuthorGuid);
                }
            }
        }

        public async Task<MessageResultDto> SendChatMessageAsync(string message, string authorGuid, string userGuid, string? chatGuid)
        {
            if ((chatGuid == null) &&
                SafetyInputHelper.CheckAntiXSSRegex(message))
            {
                ChatMessage res = await CreateChatAsync(authorGuid, userGuid, message);

                MessageResultDto dto = new()
                {
                    AuthorAvatar = res.Author.Avatar.Name,
                    AuthorGuid = res.Author.Id,
                    AuthorName = res.Author.UserName,
                    ChatGuid = res.Chat.Id,
                    Message = res.Text,
                    IsRepeat = false
                };

                await _chatContext.Clients.Group(res.Chat.Id).SendAsync("RecieveMessage", dto);

                return dto;
            }
            else 
            {
                ApplicationUser? author = await _dbContext.Users
                    .Include(x => x.Avatar)
                    .FirstOrDefaultAsync(x => x.Id.Equals(authorGuid));

                Chat? getChat = await _dbContext.Chats
                    .Include(x => x.Messages)
                    .ThenInclude(x => x.Author)
                    .FirstOrDefaultAsync(x => x.Id.Equals(chatGuid));

                if ((author != null) && (getChat != null) &&
                    SafetyInputHelper.CheckAntiXSSRegex(message))
                {
                    string? lastAG = getChat.Messages
                        .OrderBy(x => x.DateCreated)
                        .LastOrDefault()?.AuthorId;

                    bool isRepeatMessage = lastAG != null ? lastAG == authorGuid : false;

                    ChatMessage mess = new(authorGuid, message, getChat);                                      

                    await _dbContext.ChatMessages.AddAsync(mess);
                    await _dbContext.SaveChangesAsync();

                    MessageResultDto dto = new()
                    {
                        ChatGuid = getChat.Id,
                        AuthorAvatar = author.Avatar.Name,
                        AuthorGuid = author.Id,
                        AuthorName = author.UserName,
                        IsRepeat = isRepeatMessage,
                        Message = mess.Text,                        
                    };

                    await _chatContext.Clients.Group(chatGuid).SendAsync("RecieveMessage", dto);

                    return dto;
                }
            }

            return null;
        }
       
        public async Task<ChatDetailDto?> GetChatOrNullAsync(string chatGuid, string currentUserGuid)
        {
            Chat getChat = await _dbContext.Chats
                .Include(x => x.Messages)
                .ThenInclude(x => x.Idea)
                .ThenInclude(x => x.Avatar)
                .Include(x => x.Messages)
                .ThenInclude(x => x.Author)
                .ThenInclude(x => x.Avatar)
                .FirstOrDefaultAsync(x => x.Id.Equals(chatGuid));

            if (getChat != null)
            {
                // messages
                var config = new MapperConfiguration(conf => conf.CreateMap<ChatMessage, ChatMessageDto>()
                    .ForMember("IsRepost", opt => opt.MapFrom(x => x.IsRepost))
                    .ForMember("IdeaAvatar", opt => opt.MapFrom(x => x.Idea.Avatar.Name))
                    .ForMember("IdeaGuid", opt => opt.MapFrom(x => x.Idea.Id))
                    .ForMember("IsMy", opt => opt.MapFrom(x => x.AuthorId.Equals(currentUserGuid)))
                    .ForMember("AuthorGuid", opt => opt.MapFrom(x => x.AuthorId))
                    .ForMember("AuthorName", opt => opt.MapFrom(x => x.Author.UserName))
                    .ForMember("AuthorAvatar", opt => opt.MapFrom(x => x.Author.Avatar.Name))
                    .ForMember("Message", opt => opt.MapFrom(x => x.Text))
                    .ForMember("DatePublish", opt => opt.MapFrom(x => GeneratePublishDate(x.DateCreated))));                  

                var mapper = new Mapper(config);

                IEnumerable<ChatMessage> messages = getChat.Messages
                    .OrderByDescending(x => x.DateCreated)
                    .Take(20);

                ICollection<ChatMessageDto> messageDtos = mapper.Map<ICollection<ChatMessageDto>>(messages);

                FillRepeatMessages(messageDtos);

                ChatDetailDto resultDto = new()
                {
                    ChatGuid = getChat.Id,
                    CurrentUserGuid = currentUserGuid,
                    Messages = messageDtos,
                };

                return resultDto;
            }

            return null;
        }
        
        public async Task SaveChangesAsync()
            => await _dbContext.SaveChangesAsync();

        public async Task<OperationResultDto> IdeaSendJoinRequestAsync(string authorGuid, string ideaGuid)
        {
            if (ideaGuid != null)
            {
                Idea getIdea = await _dbContext.Ideas
                    .FirstOrDefaultAsync(x => x.Id.Equals(ideaGuid));

                bool getUser = _dbContext.Users
                    .Any(x => x.Id.Equals(authorGuid));

                if ((getIdea != null) && (getUser == true))
                {
                    if (!_dbContext.IdeaInvitations.Any(x => x.UserId.Equals(ideaGuid) &&
                        x.UserId.Equals(authorGuid) &&
                        x.Type.Equals(IdeaInvitationType.Join)))
                    {
                        await _dbContext.IdeaInvitations.AddAsync(new(IdeaInvitationType.Join, authorGuid, getIdea));
                        return new(true, "Заявка в идею отправлена");
                    }                    
                }                
            }

            return new(false, "При отправке запроса что-то пошло не так");
        }

        public async Task<OperationResultDto> IdeaSetReactionAsync(string reactionGuid, string ideaGuid, string userGuid)
        {
            if (!string.IsNullOrEmpty(reactionGuid) &&
                !string.IsNullOrEmpty(ideaGuid))
            {
                Idea getIdea = await _dbContext.Ideas
                    .FirstOrDefaultAsync(x => x.Id.Equals(ideaGuid));

                Reaction getReact = await _dbContext.Reactions
                    .FirstOrDefaultAsync(x => x.Id.Equals(reactionGuid));

                ApplicationUser getUser = await _dbContext.Users
                    .FirstOrDefaultAsync(x => x.Id.Equals(userGuid));

                if ((getIdea != null) && (getReact != null))
                {
                    await _dbContext.IdeaReactions.AddAsync(new(getUser, getIdea, getReact));
                    return new(true, "Реакция отправлена");
                }                
            }

            return new(false, "При отправке данных что-то пошло не так");
        }

        public async Task<ChatDetailDto?> GetActiveChatOrNullAsync(string? userGuid, string? currentUserGuid)
        {
            if ((userGuid != null) && (currentUserGuid != null) &&
                (userGuid != currentUserGuid))
            {
                Chat getChat = await _dbContext.Chats
                    .Include(x => x.Messages)
                    .ThenInclude(x => x.Author)
                    .ThenInclude(x => x.Avatar)
                    .FirstOrDefaultAsync(x => x.Users.All(x =>
                        x.UserId.Equals(userGuid) || x.UserId.Equals(currentUserGuid)));

                if (getChat != null)
                {
                    var config = new MapperConfiguration(conf => conf.CreateMap<ChatMessage, ChatMessageDto>()
                            .ForMember("IsMy", opt => opt.MapFrom(x => x.AuthorId.Equals(currentUserGuid)))
                            .ForMember("AuthorGuid", opt => opt.MapFrom(x => x.AuthorId))
                            .ForMember("AuthorName", opt => opt.MapFrom(x => x.Author.UserName))
                            .ForMember("AuthorAvatar", opt => opt.MapFrom(x => x.Author.Avatar.Name))
                            .ForMember("Message", opt => opt.MapFrom(x => x.Text))
                            .ForMember("DatePublish", opt => opt.MapFrom(x => GeneratePublishDate(x.DateCreated))));

                    IEnumerable<ChatMessage> messages = getChat.Messages
                        .OrderBy(x => x.DateCreated)
                        .Take(30);

                    ICollection<ChatMessageDto> dtos = new Mapper(config).Map<ICollection<ChatMessageDto>>(messages);

                    FillRepeatMessages(dtos);

                    return new()
                    {
                        ChatGuid = getChat.Id,
                        CurrentUserGuid = currentUserGuid,
                        Messages = dtos,
                    };
                }
            }

            return null;
        }

        public async Task<OperationResultDto> RepostIdeaAsync(string userGuid, string ideaGuid, string currentUserGuid)
        {
            Chat getChat = await _dbContext.Chats
                    .FirstOrDefaultAsync(x => x.Users.All(x =>
                        x.UserId.Equals(userGuid) || x.UserId.Equals(currentUserGuid)));

            Idea getIdea = await _dbContext.Ideas
                .Include(x => x.Avatar)
                .FirstOrDefaultAsync(x => x.Id.Equals(ideaGuid));

            ApplicationUser getAuthor = await _dbContext.Users
                .Include(x => x.Avatar)
                .FirstOrDefaultAsync(x => x.Id.Equals(currentUserGuid));

            if ((getChat != null) && (getIdea != null) && (getAuthor != null))
            {
                //IdeaRepost createRepost = new(getChat, currentUserGuid, getIdea);

                ChatMessage createRepost = new(getAuthor, getChat, getIdea);

                await _dbContext.ChatMessages.AddAsync(createRepost);
                await _dbContext.SaveChangesAsync();

                MessageResultDto dto = new()
                {
                    AuthorAvatar = getAuthor.Avatar.Name,
                    AuthorGuid = getAuthor.Id,
                    AuthorName = getAuthor.UserName,
                    ChatGuid = getChat.Id,
                    IsRepeat = false,
                    Message = getIdea.Name,
                    IdeaAvatar = getIdea.Avatar.Name,
                    IdeaGuid = getIdea.Id,
                    IsRepost = true
                };

                await _chatContext.Clients.Group(getChat.Id).SendAsync("RecieveMessage", dto);

                return new(true, "Репост отправлен");
            }
            else if ((getAuthor != null) && (getIdea != null))
            {
                Chat createChat = new Chat();
                ChatUser chatUser = new ChatUser(currentUserGuid, createChat);
                ChatUser chatUserSec = new ChatUser(userGuid, createChat);

                ChatMessage createRepost = new(getAuthor, createChat, getIdea);

                await _dbContext.ChatMessages.AddAsync(createRepost);
                
                createChat.Users.Add(chatUser);
                createChat.Users.Add(chatUserSec);

                await _dbContext.Chats.AddAsync(createChat);
                await _dbContext.SaveChangesAsync();

                MessageResultDto dto = new()
                {
                    AuthorAvatar = getAuthor.Avatar.Name,
                    AuthorGuid = getAuthor.Id,
                    AuthorName = getAuthor.UserName,
                    ChatGuid = createChat.Id,
                    IsRepeat = false,
                    Message = getIdea.Name,
                    IdeaAvatar = getIdea.Avatar.Name,
                    IdeaGuid = getIdea.Id,
                    IsRepost = true
                };

                await _chatContext.Clients.Group(createChat.Id).SendAsync("RecieveMessage", dto);

                return new(true, "Success");
            }

            return new(false, "При отправлении репоста что-то пошло не так");
        }

        public IEnumerable<IdeaToInviteDto> GetUserIdeasToInvite(string userGuid)
        {
            if (userGuid != null)
            {
                var ideas = _dbContext.Ideas
                    .Include(x => x.Members)
                    .Where(x => x.Members.Any(x => x.UserId.Equals(userGuid) &&
                    x.Role.Equals(IdeaMemberRoles.Author)))
                    .OrderByDescending(x => x.DateCreated)
                    .Take(30);

                var config = new MapperConfiguration(conf => conf.CreateMap<Idea, IdeaToInviteDto>()
                    .ForMember("IdeaGuid", opt => opt.MapFrom(x => x.Id))
                    .ForMember("IdeaName", opt => opt.MapFrom(x => x.Name)));

                IEnumerable<IdeaToInviteDto> dtos = new Mapper(config).Map<IEnumerable<IdeaToInviteDto>>(ideas);

                return dtos;
            }
            else return null;
        }

        public async Task<OperationResultDto> SendIdeaInviteAsync(string currentUserGuid, string userGuid, string ideaGuid)
        {
            if (!string.IsNullOrEmpty(currentUserGuid) &&
                !string.IsNullOrEmpty(userGuid) &&
                !string.IsNullOrEmpty(ideaGuid))
            {
                IdeaInvitation getOrCheckExist = await _dbContext.IdeaInvitations
                    .FirstOrDefaultAsync(x => x.Idea.Id.Equals(ideaGuid) &&
                        x.UserId.Equals(userGuid));

                Idea isRealAuthorIdea = await _dbContext.Ideas
                    .Include(x => x.Members)
                    .FirstOrDefaultAsync(x => x.Members.Any(x => x.UserId.Equals(currentUserGuid) &&
                        x.Role.Equals(IdeaMemberRoles.Author) &&
                        x.Idea.Id.Equals(ideaGuid)));

                if (isRealAuthorIdea != null)
                {
                    if (getOrCheckExist != null)
                    {
                        getOrCheckExist.DateCreated = DateTime.Now;
                        _dbContext.IdeaInvitations.Update(getOrCheckExist);
                    }
                    else
                    {
                        IdeaInvitation create = new(IdeaInvitationType.Invite, userGuid, isRealAuthorIdea);
                        await _dbContext.IdeaInvitations.AddAsync(create);
                    }
                    await _dbContext.SaveChangesAsync();

                    return new(true, "Приглашение в идею отправлено");
                }
            }
            return new(false, "При отправке запроса что-то пошло не так");
        }

        public async Task<TopicDetail> GetTopicDetailOrNullAsync(string currentUserGuid, string topicGuid)
        {
            if (!string.IsNullOrWhiteSpace(currentUserGuid) &&
                !string.IsNullOrWhiteSpace(topicGuid))
            {
                var getTopic = await _dbContext.IdeaTopics
                    .Include(x => x.Idea)
                    .ThenInclude(x => x.Members)
                    .Include(x => x.Author)
                    .ThenInclude(x => x.Avatar)
                    .Include(x => x.Comments)
                    .ThenInclude(x => x.Author)
                    .ThenInclude(x => x.Avatar)
                    .FirstOrDefaultAsync(x => x.Id.Equals(topicGuid));

                getTopic.Comments = getTopic.Comments
                    .OrderByDescending(x => x.DateCreated)
                    .Take(30)
                    .ToList();

                var config = new MapperConfiguration(conf => conf.CreateMap<IdeaTopic, TopicDetail>()
                    .ForMember("Guid", opt => opt.MapFrom(x => x.Id))
                    .ForMember("Name", opt => opt.MapFrom(x => x.Name))
                    .ForMember("Description", opt => opt.MapFrom(x => x.Description))
                    .ForMember("DatePublished", opt => opt.MapFrom(x =>
                        IdeaHelper.NormalizeDate(x.DateCreated)))
                    .ForMember("AuthorGuid", opt => opt.MapFrom(x => x.AuthorId))
                    .ForMember("AuthorAvatar", opt => opt.MapFrom(x => x.Author.Avatar.Name))
                    .ForMember("CanEdit", opt => opt.MapFrom(x =>
                        IdeaHelper.CheckUserCanEditIdeaObject(x.AuthorId, currentUserGuid, x.Idea.Members, x.IsInit)))
                    .ForMember("ByModder", opt => opt.MapFrom(x => x.IsDefault))
                    .ForMember("Comments", opt => opt.MapFrom(x => getTopic.Comments
                        .OrderByDescending(x => x.DateCreated)
                        .Take(30)
                        .Select(e => new TopicDetailCommentDto()
                        { 
                            AuthorAvatar = e.Author.Avatar.Name,
                            AuthorGuid = e.AuthorId,
                            AuthorName = e.Author.UserName,
                            Comment = e.Message,
                            DatePublished = IdeaHelper.NormalizeDate(e.DateCreated),
                            Guid = e.Id
                        }))));

                var mapper = new Mapper(config);

                TopicDetail dto = mapper.Map<TopicDetail>(getTopic);

                return dto;
            }

            return null;
        }

        public async Task<OperationResultDto> CreateTopicCommentAsync(string authorGuid, string topicGuid, string text)
        {
            if (!string.IsNullOrWhiteSpace(text) &&
                SafetyInputHelper.CheckAntiXSSRegex(text))
            {
                await _dbContext.IdeaTopicComments.AddAsync(new(topicGuid, authorGuid, text));
                await _dbContext.SaveChangesAsync();

                return new(true, "Op. Success");
            }
            return new(true, "Op. Failed");
        }

        public async Task<OperationResultDto> CreateTopicAsync(string name, string content, string ideaGuid, string authorGuid)
        {
            if (!string.IsNullOrEmpty(authorGuid) &&
                !string.IsNullOrWhiteSpace(name) &&
                !string.IsNullOrWhiteSpace(content) &&
                !string.IsNullOrWhiteSpace(ideaGuid))
            {
                Idea idea = await _dbContext.Ideas
                    .Include(x => x.Members)
                    .FirstOrDefaultAsync(x => x.Id.Equals(ideaGuid));

                if (idea != null)
                {
                    IdeaMemberRoles? getRole = idea.Members
                        .FirstOrDefault(x => x.UserId.Equals(authorGuid))
                        ?.Role;                    

                    if ((getRole != null) &&
                        SafetyInputHelper.CheckAntiXSSRegex(name) &&
                        SafetyInputHelper.CheckAntiXSSRegex(content))
                    {
                        await _dbContext.IdeaTopics.AddAsync(new(idea, authorGuid, name, content, getRole <= IdeaMemberRoles.Modder, false));
                        await _dbContext.SaveChangesAsync();
                        return new(true, "Op. (Create) Success");
                    }
                }
            }
            return new(false, "Op. (Create) Failed");
        }

        public async Task<OperationResultDto> RemoveTopicCommentAsync(string commentGuid, string topicGuid, string currentUserGuid)
        {
            IdeaTopic getTopic = await _dbContext.IdeaTopics
                .Include(x => x.Comments)
                .Include(x => x.Idea)
                .ThenInclude(x => x.Members)
                .Include(x => x.Author)
                .FirstOrDefaultAsync(x => x.Id.Equals(topicGuid));

            IdeaTopicComment getComment = getTopic?.Comments
                .FirstOrDefault(x => x.Id.Equals(commentGuid));

            if (getComment != null)
            {
                bool checkCanUserEdit = IdeaHelper.CheckUserCanEditIdeaObject(
                getTopic.Author.Id, currentUserGuid, getTopic.Idea.Members, getTopic.IsInit);

                if (checkCanUserEdit)
                {
                    _dbContext.IdeaTopicComments.Remove(getComment);
                    await _dbContext.SaveChangesAsync();
                    return new(true, "Op. (Remove) Success");
                }
            }            
            return new(false, "Op. (Remove) Failed");
        }

        public async Task<OperationResultDto> RemoveTopicAsync(string topicGuid, string currentUserGuid)
        {
            IdeaTopic getTopic = await _dbContext.IdeaTopics
                .Include(x => x.Comments)
                .Include(x => x.Idea)
                .ThenInclude(x => x.Members)
                .Include(x => x.Author)
                .FirstOrDefaultAsync(x => x.Id.Equals(topicGuid));

            if (IdeaHelper.CheckUserCanEditIdeaObject(
                getTopic.Author.Id, currentUserGuid, getTopic.Idea.Members, getTopic.IsInit))
            {
                _dbContext.IdeaTopics.Remove(getTopic);
                await _dbContext.SaveChangesAsync();
                return new(true, "Op. (Remove) Success");
            }
            return new(false, "Op. (Remove) Failed");
        }        

        public async Task<OperationResultDto> CreateGoalAsync(string name, string description, string ideaGuid, bool withTasks, string authorGuid)
        {
            if (!string.IsNullOrEmpty(ideaGuid) &&
                !string.IsNullOrEmpty(authorGuid))
            {
                Idea getIdea = await _dbContext.Ideas
                    .FirstOrDefaultAsync(x => x.Id.Equals(ideaGuid));

                ApplicationUser getUser = await _dbContext.Users
                    .FirstOrDefaultAsync(x => x.Id.Equals(authorGuid));

                if ((getIdea != null) && (getUser != null) &&
                    SafetyInputHelper.CheckAntiXSSRegex(name) &&
                    SafetyInputHelper.CheckAntiXSSRegex(description))
                {
                    IdeaGoal create = new(getIdea, getUser, name, description, false, false);

                    if (withTasks)
                    {
                        IdeaGoalTask task1 = new(getUser.Id, IdeaGoalTaskType.Waiting, "Создать 5 топиков");
                        IdeaGoalTask task2 = new(getUser.Id, IdeaGoalTaskType.Waiting, "Добавьте 5 новых задач");
                        create.Tasks.Add(task1);
                        create.Tasks.Add(task2);
                    }

                    await _dbContext.IdeaGoals.AddAsync(create);
                    await _dbContext.SaveChangesAsync();

                    return new(true, "Op. (Create) Success");
                }
            }

            return new(false, "Opt. (Create) Failed");
        }
              
        public async Task<OperationResultDto> RemoveIdeaMemberAsync(string ideaGuid, string userGuid, string curUserGuid)
        {
            if (!string.IsNullOrWhiteSpace(ideaGuid) &&
                !string.IsNullOrWhiteSpace(userGuid) &&
                !string.IsNullOrWhiteSpace(curUserGuid))
            {
                Idea getIdea = await _dbContext.Ideas
                    .Include(x => x.Invitations)
                    .Include(x => x.Members)
                    .FirstOrDefaultAsync(x => x.Id.Equals(ideaGuid));

                if (getIdea != null && 
                    IdeaHelper.CheckUserIsHavingIdeaRole(getIdea.Members, curUserGuid, IdeaMemberRoles.Author))
                {
                    IdeaMember getMember = getIdea.Members
                        .FirstOrDefault(x => x.UserId.Equals(userGuid));

                    if (getMember != null)
                    {
                        _dbContext.IdeaMembers.Remove(getMember);
                        await _dbContext.SaveChangesAsync();

                        return new(true, "Op. Remove member (Success)");
                    }
                }
            }

            return new(false, "Op. Remove member (Failed)");
        }

        public async Task<OperationResultDto> RejectIdeaMemberRequestAsync(string ideaGuid, string userGuid, string curUserGuid)
        {
            Idea getIdea = await _dbContext.Ideas
                    .Include(x => x.Invitations)
                    .Include(x => x.Members)
                    .FirstOrDefaultAsync(x => x.Id.Equals(ideaGuid));

            if (getIdea != null &&
                IdeaHelper.CheckUserIsHavingIdeaRole(getIdea.Members, curUserGuid, IdeaMemberRoles.Modder))
            {
                IdeaInvitation getRequest = getIdea.Invitations
                    .FirstOrDefault(x => x.UserId.Equals(userGuid));

                if (getRequest != null)
                {
                    _dbContext.IdeaInvitations.Remove(getRequest);
                    await _dbContext.SaveChangesAsync();

                    return new(true, "Op. Reject member req. (Success)");
                }
            }

            return new(false, "Op. Reject member req. (Failed)");
        }

        public async Task<OperationResultDto> AcceptIdeaMemberRequestAsync(string ideaGuid, string userGuid, string curUserGuid)
        {
            Idea getIdea = await _dbContext.Ideas
                    .Include(x => x.Invitations)
                    .Include(x => x.Members)
                    .FirstOrDefaultAsync(x => x.Id.Equals(ideaGuid));

            if (getIdea != null && 
                IdeaHelper.CheckUserIsHavingIdeaRole(getIdea.Members, curUserGuid, IdeaMemberRoles.Modder))
            {
                IdeaInvitation getRequest = getIdea.Invitations
                    .FirstOrDefault(x => x.UserId.Equals(userGuid));

                if (getRequest != null)
                {
                    IdeaMember createMember = new(userGuid, IdeaMemberRoles.Member);

                    getIdea.Members.Add(createMember);

                    _dbContext.Ideas.Update(getIdea);
                    _dbContext.IdeaInvitations.Remove(getRequest);
                    await _dbContext.SaveChangesAsync();

                    return new(true, "Op. Accept member req. (Success)");
                }
            }

            return new(false, "Op. Accept member req. (Failed)");
        }

        public async Task<OperationResultDto> CreateGoalTaskAsync(string content, string ideaGuid, string goalGuid, string currentUserGuid)
        {
            Idea getIdea = await _dbContext.Ideas
                .Include(x => x.Goals)
                .Include(x => x.Members)
                .FirstOrDefaultAsync(x => x.Id.Equals(ideaGuid));

            if (getIdea != null)
            {
                if (IdeaHelper.CheckUserIsHavingIdeaRole(
                    getIdea.Members, currentUserGuid, IdeaMemberRoles.Modder))
                {
                    IdeaGoal getGoal = getIdea.Goals
                        .FirstOrDefault(x => x.Id.Equals(goalGuid));

                    if (getGoal != null && SafetyInputHelper.CheckAntiXSSRegex(content))
                    {
                        IdeaGoalTask createTask = new(currentUserGuid, IdeaGoalTaskType.Waiting, content);
                        getGoal.Tasks.Add(createTask);
                        _dbContext.IdeaGoals.Update(getGoal);
                        await _dbContext.SaveChangesAsync();

                        return new(true, "Op. create goal task (Success)");
                    }
                }
            }

            return new(false, "Op. create goal task (Failed)");
        }        

        public async Task<GoalDetailDto> GetGoalDetailOrNullAsync(string currentUserGuid, string goalGuid)
        {
            var getGoal = await _dbContext.IdeaGoals
                    .Include(x => x.Idea)
                    .ThenInclude(x => x.Members)
                    .Include(x => x.Author)
                    .ThenInclude(x => x.Avatar)
                    .Include(x => x.Tasks)
                    .ThenInclude(x => x.Author)
                    .ThenInclude(x => x.Avatar)
                    .FirstOrDefaultAsync(x => x.Id.Equals(goalGuid));

            if (getGoal != null)
            {
                getGoal.Tasks = getGoal.Tasks
                .OrderByDescending(x => x.Type)
                .Take(30)
                .ToList();

                var config = new MapperConfiguration(conf => conf.CreateMap<IdeaGoal, GoalDetailDto>()
                    .ForMember("Guid", opt => opt.MapFrom(x => x.Id))
                    .ForMember("Name", opt => opt.MapFrom(x => x.Name))
                    .ForMember("Description", opt => opt.MapFrom(x => x.Description))
                    .ForMember("DatePublished", opt => opt.MapFrom(x =>
                        IdeaHelper.NormalizeDate(x.DateCreated)))
                    .ForMember("AuthorGuid", opt => opt.MapFrom(x => x.AuthorId))
                    .ForMember("AuthorAvatar", opt => opt.MapFrom(x => x.Author.Avatar.Name))
                    .ForMember("CanEdit", opt => opt.MapFrom(x =>
                        IdeaHelper.CheckUserCanEditIdeaObject(x.AuthorId, currentUserGuid, x.Idea.Members, false)))
                    .ForMember("Tasks", opt => opt.MapFrom(x => getGoal.Tasks
                        .OrderByDescending(x => x.DateCreated)
                        .Take(30)
                        .Select(e => new GoalTaskDto()
                        {
                            AuthorAvatar = e.Author.Avatar.Name,
                            AuthorGuid = e.AuthorId,
                            AuthorName = e.Author.UserName,
                            Description = e.Content,
                            DatePublished = IdeaHelper.NormalizeDate(e.DateCreated),
                            Guid = e.Id,
                            Status = e.Type
                        }))));

                return new Mapper(config).Map<GoalDetailDto>(getGoal);                
            }

            return null;
        }

        public async Task<OperationResultDto> ChangeGoalTaskStatusAsync(string currentUserGuid, string goalGuid, string taskGuid, IdeaGoalTaskType newStatus)
        {
            IdeaGoal getGoal = await _dbContext.IdeaGoals
                .Include(x => x.Idea)
                .ThenInclude(x => x.Members)
                .Include(x => x.Tasks)
                .FirstOrDefaultAsync(x => x.Id.Equals(goalGuid));

            if ((getGoal != null) && (getGoal.Idea.Members != null))
            {
                if (IdeaHelper.CheckUserIsHavingIdeaRole(
                    getGoal.Idea?.Members, currentUserGuid, IdeaMemberRoles.Modder))
                {
                     IdeaGoalTask getTask = getGoal.Tasks
                        .FirstOrDefault(x => x.Id.Equals(taskGuid));

                    if (getTask != null)
                    {
                        getTask.Type = newStatus;
                        _dbContext.IdeaGoalTasks.Update(getTask);
                        await _dbContext.SaveChangesAsync();
                        return new(true, "Update Task Type status (Success)");
                    }
                }
            }
            return new(false, "Update Task Type status (Failed)");
        }

        public async Task<OperationResultDto> UpdateIdeaAsync(UpdateIdeaModel model, string currentUserGuid)
        {
            Idea getIdea = await _dbContext.Ideas
                .Include(x => x.Tags)
                .Include(x => x.Status)
                .Include(x => x.Avatar)
                .Include(x => x.Topics)
                .Include(x => x.Members)
                .FirstOrDefaultAsync(x => x.Id.Equals(model.Idea));

            if (getIdea != null)
            {
                if (IdeaHelper.CheckUserIsHavingIdeaRole(
                    getIdea.Members, currentUserGuid, IdeaMemberRoles.Author) && 
                    SafetyInputHelper.CheckAntiXSSRegex(model.Description))
                {
                    if (!string.IsNullOrWhiteSpace(model.Description))
                    {
                        var getTopic = getIdea.Topics
                            .OrderByDescending(x => x.IsInit)
                            .OrderByDescending(x => x.DateCreated)
                            .FirstOrDefault();
                        getTopic.Description = model.Description;
                    }

                    if ((model.Avatar != null) && 
                        (model.Avatar.FileName != AvatarInformation.IdeaDefaultAvatarName))
                    {
                        using (FileStream str = new($"wwwroot/media/ideaAvatars/" + model.Avatar.FileName, FileMode.Create))
                        {
                            await model.Avatar.CopyToAsync(str);
                            str.Close();
                        }

                        if ((getIdea.Avatar.Name != AvatarInformation.IdeaDefaultAvatarName) &&
                            !getIdea.Avatar.IsDefault)
                            File.Delete($"wwwroot/media/ideaAvatars/{getIdea.Avatar.Name}");

                        getIdea.Avatar = new IdeaAvatar(false, model.Avatar.FileName);
                    }

                    if (model.Tags.Count > 0)
                    {
                        List<Tag> updatedTags = new();
                        foreach (var tag in model.Tags)
                        {
                            Tag? getTag = await _dbContext.Tags
                                .FirstOrDefaultAsync(x => x.Id.Equals(tag));

                            if (getTag != null)
                                updatedTags.Add(getTag);                           
                        }

                        getIdea.Tags = updatedTags;
                    }

                    IdeaStatus updatedStatus = await _dbContext.IdeaStatuses
                        .FirstOrDefaultAsync(x => x.Type.Equals(model.Status));

                    if (updatedStatus != null)
                        getIdea.Status = updatedStatus;

                    getIdea.IsPrivate = model.Private;

                    _dbContext.Ideas.Update(getIdea);
                    await _dbContext.SaveChangesAsync();

                    return new(true, "Update Idea (Success)");
                }
            }
            return new(false, "Update Idea (Failed)");
        }

        public async Task<OperationResultDto> SetIdeaMemberRoleAsync(string ideaGuid, string userGuid, string currentUserGuid, IdeaRolesToUpdate newRole)
        {
            Idea getIdea = await _dbContext.Ideas
                .Include(x => x.Members)
                .FirstOrDefaultAsync(x => x.Id.Equals(ideaGuid));

            if (getIdea != null)
            {
                if (IdeaHelper.CheckUserIsHavingIdeaRole(
                    getIdea.Members, currentUserGuid, IdeaMemberRoles.Author))
                {
                    IdeaMember getMember = getIdea.Members
                        .FirstOrDefault(x => x.UserId.Equals(userGuid));

                    if (getMember != null)
                    {
                        IdeaMemberRoles normalizeRole = IdeaMemberRoles.Member;
                        if (newRole == IdeaRolesToUpdate.Modder)
                            normalizeRole = IdeaMemberRoles.Modder;
                        if (newRole == IdeaRolesToUpdate.Member)
                            normalizeRole = IdeaMemberRoles.Member;

                        getMember.Role = normalizeRole;
                        _dbContext.IdeaMembers.Update(getMember);
                        await _dbContext.SaveChangesAsync();

                        return new(true, "Op. Update Role (Success)");
                    }
                }
            }

            return new(false, "Op. Update Role (Failed)");
        }

        public async Task<OperationResultDto> RemoveIdeaAsync(string ideaGuid, string userGuid, string userPassword)
        {
            Idea getIdea = await _dbContext.Ideas
                .Include(x => x.Members)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id.Equals(ideaGuid));

            if (getIdea != null)
            {
                if (IdeaHelper.CheckUserIsHavingIdeaRole(
                    getIdea.Members, userGuid, IdeaMemberRoles.Author))
                {
                    ApplicationUser getAuthor = getIdea.Members
                        .FirstOrDefault(x => x.UserId.Equals(userGuid))
                        .User;

                    if (await _userManager.CheckPasswordAsync(getAuthor, userPassword))
                    {
                        Idea loadFullIdea = await _dbContext.Ideas
                            .Include(x => x.Reactions)
                            .Include(x => x.Members)
                            .Include(x => x.Invitations)
                            .Include(x => x.Reposts)
                            .Include(x => x.Topics)                           
                            .ThenInclude(x => x.Comments)
                            .Include(x => x.Goals)
                            .ThenInclude(x => x.Tasks)
                            .Include(x => x.Avatar)
                            .Include(x => x.Contact)
                            .FirstOrDefaultAsync(x => x.Id.Equals(ideaGuid));

                        _dbContext.Ideas.Remove(loadFullIdea);                    
                        await _dbContext.SaveChangesAsync();

                        if (loadFullIdea.Avatar.Name != AvatarInformation.IdeaDefaultAvatarName)
                            File.Delete("wwwroot/media/ideaAvatars/" + loadFullIdea.Avatar.Name);

                        return new(true, "Op. Remove idea (Success)");
                    }
                }
            }

            return new(false, "Op. Remove idea (Failed)");
        }
    }
}
