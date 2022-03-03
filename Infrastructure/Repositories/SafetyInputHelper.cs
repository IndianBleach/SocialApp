using Infrastructure.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public static class SafetyInputHelper
    {
        public static bool CheckAntiXSSRegex(string inputText)
            => !Regex.IsMatch(inputText, ConstantsHelper.XSSCheckRegexPattern);
    }
}
