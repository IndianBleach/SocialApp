﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.User
{
    public class ActiveChatUserDto
    {
        public string? ChatGuid { get; set; }
        public string UserGuid { get; set; }
        public string UserName { get; set; }
        public string UserAvatar { get; set; }        
    }
}
