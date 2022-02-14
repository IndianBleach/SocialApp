using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.Authorization
{
    public class AuthorizationResultDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public AuthorizationResultDto(bool success, string message)
        {
            IsSuccess = success;
            Message = message;
        }
    }
}
