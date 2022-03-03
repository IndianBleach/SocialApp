using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Constants
{
    public static class ConstantsHelper
    {
        public const int CountIdeasPerPage = 10;

        public const int CountUsersPerPage = 10;

        public const int OrderByCount = 10;

        public const string XSSCheckRegexPattern = @"((alert|on\w+|function\s+\w+)\s*\(\s*(['+\d\w](,?\s*['+\d\w]*)*)*\s*\))|(<(script|iframe|embed|frame|frameset|object|img|applet|body|html|style|layer|link|ilayer|meta|bgsound))";
    }
}
