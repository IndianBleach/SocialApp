﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.Create
{
    public class CreateIdeaDto
    {
        public string? AuthorGuid { get; set; }
        [Required]
        [MaxLength(36, ErrorMessage = "Максимум 36 символов")]
        [MinLength(4, ErrorMessage = "Минимум 4 символа")]
        public string Name { get; set; }
        [Required]
        public string Description {  get; set; }
        [Required]
        public ICollection<string> Tags { get; set; }
        public bool IsPrivate { get; set; }
    }
}
