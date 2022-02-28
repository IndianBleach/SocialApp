using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs.User
{
    public class UpdateGeneralSettingsDto
    {
        public IFormFile Avatar { get; set; }
        public string? Description { get; set; }
        public string? ContactName { get; set; }
        public string? ContactUrl { get; set; }
        public string? AddressCountry { get; set; }
        public string? AddressCity { get; set; }
    }
}
