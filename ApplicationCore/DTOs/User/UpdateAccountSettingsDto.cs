using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.User
{
    public class UpdateAccountSettingsDto
    {
        public string? Username { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "Минимум 1 тег")]
        public ICollection<string> Tags { get; set; }
        public string? OldPassword { get; set; }
        [DataType(DataType.Password)]        
        public string? NewPassword { get; set; }
        [Compare("NewPassword")]
        public string? NewPasswordConfirm { get; set; }
    }
}
