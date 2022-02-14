using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.Create
{
    public class CreateOperationResult
    {
        public string Message { get; set; }
        public string? CreatedObjectGuid { get; set; }
        public bool IsSuccess { get; set; }

        public CreateOperationResult(bool isSuccess, string? guid, string message)
        {
            Message = message;
            CreatedObjectGuid = guid;
            IsSuccess = isSuccess;
        }
    }
}
