using ApplicationCore.ChatHub;
using ApplicationCore.DTOs.AsyncLoad;
using ApplicationCore.DTOs.AsyncLoad.Chat;
using ApplicationCore.Entities.Chat;
using ApplicationCore.Entities.IdeaEntity;
using ApplicationCore.Entities.UserEntity;
using ApplicationCore.Identity;
using ApplicationCore.Interfaces;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AsyncLoadService : IAsyncLoadService
    {
        private readonly ApplicationContext _dbContext;
        private IHubContext<ChatHub> _chatContext;

        public AsyncLoadService(ApplicationContext context, IHubContext<ChatHub> chatContext)
        {
            _dbContext = context;
            _chatContext = chatContext;
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

            if ((getUser != null) || (getUserSec != null))
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

            List<SmallUserDto> dtos = new List<SmallUserDto>();

            foreach (var friend in getFriends)
            {
                var dtoFriend = friend.Users.FirstOrDefault(x => x.Id != guid);

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

            return dtos;
        }

        private async Task<ChatMessage> CreateChatAsync(string authorGuid, string userSecGuid, string message)
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
            if (chatGuid == null)
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

                if ((author != null) && (getChat != null))
                {
                    string lastAG = getChat.Messages
                        .OrderBy(x => x.DateCreated)
                        .Last().AuthorId;

                    bool isRepeatMessage = lastAG == authorGuid;

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

                ApplicationUser getUser = await _dbContext.Users
                    .FirstOrDefaultAsync(x => x.Id.Equals(authorGuid));

                if ((getIdea != null) && (getUser != null))
                {
                    IdeaInvitation createInvite = new(IdeaInvitationType.Join, authorGuid, getIdea);
                    await _dbContext.IdeaInvitations.AddAsync(createInvite);

                    return new(true, "Заявка в идею отправлена");
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
                    IdeaReaction createReact = new(getUser, getIdea, getReact);
                    await _dbContext.IdeaReactions.AddAsync(createReact);

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

                    var mapper = new Mapper(config);

                    IEnumerable<ChatMessage> messages = getChat.Messages
                        .OrderBy(x => x.DateCreated)
                        .Take(30);

                    ICollection<ChatMessageDto> dtos = mapper.Map<ICollection<ChatMessageDto>>(messages);

                    FillRepeatMessages(dtos);

                    ChatDetailDto resultDto = new()
                    {
                        ChatGuid = getChat.Id,
                        CurrentUserGuid = currentUserGuid,
                        Messages = dtos,
                    };

                    return resultDto;
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

                var mapper = new Mapper(config);

                IEnumerable<IdeaToInviteDto> dtos = mapper.Map<IEnumerable<IdeaToInviteDto>>(ideas);

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
    }
}
