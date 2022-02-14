using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.AsyncLoad.Chat
{
    public class MessageResultDto
    {
        public bool IsRepeat { get; set; }
        public string ChatGuid { get; set; }
        public string AuthorGuid { get; set; }
        public string AuthorName {  get; set; }
        public string AuthorAvatar { get; set; }
        public string Message {  get; set; }
    }
}
