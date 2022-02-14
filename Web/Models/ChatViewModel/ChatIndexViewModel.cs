using ApplicationCore.DTOs.Tag;
using ApplicationCore.DTOs.User;
using System.Collections.Generic;

namespace WebUi.Models.ChatViewModel
{
    public class ChatIndexViewModel
    {
        public ICollection<ChatUserDto> ChatUsers { get; set; }
        public string Guid { get; set; }
        public List<TagDto> Tags { get; set; }
        public bool IsAuthorized { get; set; }
    }
}
