using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.AsyncLoad.Chat
{

    public class ChatMessageDto
    {        
        public bool IsRepost { get; set; }
        public string? IdeaGuid { get; set; }
        public string? IdeaAvatar { get; set; }

        public bool IsMy { get; set; }
        public bool IsRepeat { get; set; }
        public string AuthorGuid { get; set; }
        public string AuthorName { get; set; }
        public string AuthorAvatar { get; set; }
        public string Message { get; set; }
        public string DatePublish { get; set; }
    }



    public class ChatDetailDto
    {
        public string CurrentUserGuid { get; set; }
        public string ChatGuid { get; set; }
        public ICollection<ChatMessageDto> Messages { get; set; }

    }
}
