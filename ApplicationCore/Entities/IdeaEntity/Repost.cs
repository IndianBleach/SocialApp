using ApplicationCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.IdeaEntity
{
    public class IdeaRepost : BaseEntity
    {
        public string ChatId { get; set; }
        public Chat.Chat Chat { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string IdeaId { get; set; }
        public Idea Idea { get; set; }
        public DateTime DateCreated { get; set; }

        public IdeaRepost(string chatId, string userId, string ideaId)
        {
            ChatId = chatId;
            UserId = userId;
            IdeaId = ideaId;
            DateCreated = DateTime.Now;
        }

        public IdeaRepost(Chat.Chat chat, string userId, Idea idea)
        {
            Chat = chat;
            UserId = userId;
            Idea = idea;
            DateCreated = DateTime.Now;
        }
    }
}
