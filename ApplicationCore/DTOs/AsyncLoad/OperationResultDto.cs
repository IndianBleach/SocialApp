using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.AsyncLoad
{
    public class OperationResultDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public OperationResultDto(bool success, string message)
        {
            IsSuccess = success;
            Message = message;
        }
    }
}
