using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace WebUi.Controllers.Extensions
{
    public class ExtendedController : Controller
    {
        public string? GetUserIdOrNull()
        {
            var claim = User.Identities.First().Claims
                .FirstOrDefault(x => x.Type.Equals("UserId"));

            string? userGuid = claim != null ? claim.Value : null;

            return userGuid;
        }

        public bool IsUserAuthenticated()
        {
            if (User.Identity != null)
                return User.Identity.IsAuthenticated;

            return false;
        }
    }
}
