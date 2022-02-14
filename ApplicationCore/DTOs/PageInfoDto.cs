using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.DTOs
{
    public class PageInfoDto
    {
        public bool IsActive;
        public int NumberValue;

        public PageInfoDto(bool isActive, int number)
        {
            IsActive = isActive;
            NumberValue = number;
        }
    }
}
