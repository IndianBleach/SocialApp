using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.Authorization
{
    public class UserSignInDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class UserSignUpDto
    {
        [Required]
        [MinLength(4, ErrorMessage = "Минимум 4 символа")]
        [MaxLength(22, ErrorMessage = "Максимум 22 символа")]
        public string Username { get; set; }
        [Required]
        [MinLength(4, ErrorMessage = "Минимум 4 символа")]
        [MaxLength(22, ErrorMessage = "Максимум 22 символа")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "Выберите минимум 1 тег")]
        [MaxLength(5, ErrorMessage = "Максимум 5 тегов")]
        public ICollection<string> Tags { get; set; }
    }
}
