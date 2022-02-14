using ApplicationCore.Entities.IdeaEntity;
using ApplicationCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.Chat
{
    public class ChatMessage : BaseEntity
    {
        public string AuthorId { get; set; }
        public ApplicationUser Author { get; set; }
        public string Text { get; set; }
        public DateTime DateCreated { get; set; }
        public Chat Chat { get; set; }

        public ChatMessage(ApplicationUser author, string text, Chat chat)
        {
            Author = author;
            Text = text;
            DateCreated = DateTime.Now;
            Chat = chat;
        }

        public ChatMessage(string authorId, string text)
        {
            AuthorId = authorId;
            Text = text;
            DateCreated = DateTime.Now;
        }

        public ChatMessage(string authorId, string text, Chat chat)
        {
            AuthorId = authorId;
            Text = text;
            DateCreated = DateTime.Now;
            Chat = chat;
        }
    }


    public class ChatUser : BaseEntity
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public Chat Chat { get; set; }

        public ChatUser(string userId)
        {
            UserId = userId;
        }

        public ChatUser(string userId, Chat chat)
        {
            UserId = userId;
            Chat = chat;
        }
    }

    public class Chat : BaseEntity
    {
        public ICollection<ChatUser> Users { get; set; }
        public ICollection<ChatMessage> Messages { get; set; }
        public ICollection<IdeaRepost> Reposts { get; set; }

        public Chat()
        {
            Messages = new List<ChatMessage>();
            Users = new List<ChatUser>();
            Reposts = new List<IdeaRepost>();
        }
    }
}
