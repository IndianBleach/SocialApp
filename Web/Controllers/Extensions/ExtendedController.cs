using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace WebUi.Controllers.Extensions
{
    public class ExtendedController : Controller
    {
        public string? GetUserIdOrNull()
        {
            return User.Identities.First().Claims
                .FirstOrDefault(x => x.Type.Equals("UserId"))?.Value;
        }

        public bool IsUserAuthenticated()
        {
            if (User.Identity != null)
                return User.Identity.IsAuthenticated;

            return false;
        }
    }
}
