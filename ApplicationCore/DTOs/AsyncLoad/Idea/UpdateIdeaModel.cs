using ApplicationCore.Entities.IdeaEntity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.AsyncLoad.Idea
{
    public class UpdateIdeaModel
    {
        public IFormFile Avatar { get; set; }
        public string Description { get; set; }
        [Required]
        public IdeaStatusType Status { get; set; }
        public bool Private { get; set; }
        [Required]
        public string Idea { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "Нужен минимум 1 тег")]
        public ICollection<string> Tags { get; set; }
    }
}
