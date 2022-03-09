using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Controllers
{
    public class AboutController : Controller
    {
        [Route("/about")]
        public IActionResult Index(string? section)
        {
            string[] allSections = new string[]
            {
                "ideas",
                "communication",
                "contacts",
                "us",
            };

            string validSection = allSections.Any(x => x.Equals(section?.ToLower())) == true
                ? section?.ToLower() : "ideas";

            return View(validSection);
        }
    }
}
