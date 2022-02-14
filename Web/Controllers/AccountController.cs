using ApplicationCore.DTOs.Authorization;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebUi.Controllers.Extensions;
using WebUi.Models;

namespace WebUi.Controllers
{
    public class AccountController : ExtendedController
    {
        private readonly ApplicationCore.Interfaces.IAuthorizationService _authorizationService;
        private readonly ITagService _tagService;

        public AccountController(ApplicationCore.Interfaces.IAuthorizationService authService,
            ITagService tagService)
        {
            _authorizationService = authService;
            _tagService = tagService;
        }

        private async Task<bool> TestFunc(UserSignUpDto model)
        {
            var result = await _authorizationService.UserSignUpAsync(model);

            return result.IsSuccess;
        }

        public IActionResult Test()
        {
            var res = _authorizationService.CreateResult(false, "Введены некорректные данные");

            return View("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            if (IsUserAuthenticated())
                return RedirectToAction("index", "home");

            DemoViewModel indexVm = new()
            {
                Tags = await _tagService.GetAllTagsAsync()
            };

            return View(indexVm);
        }        

        [Route("account/signout")]
        public async Task<IActionResult> SignOut()
        {
            await _authorizationService.UserSignOutAsync();

            return RedirectToAction("login", "account");
        }


        [HttpPost]
        [Route("account/signup")]
        public async Task<JsonResult> SignUp(UserSignUpDto model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authorizationService.UserSignUpAsync(model);

                if (result.IsSuccess)
                {
                    return Json(new
                    {
                        redirectUrl = Url.Action("Index", "Home"),
                        isRedirect = true
                    });
                }
            }

            var res = _authorizationService.CreateResult(false, "Введены некорректные данные");
            return Json(res);
        }

        [HttpPost]
        [Route("account/signin")]
        public async Task<JsonResult> SignIn(UserSignInDto model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authorizationService.UserSignInAsync(model);

                if (result.IsSuccess)
                {
                    return Json(new
                    {
                        redirectUrl = Url.Action("Index", "Home"),
                        isRedirect = true
                    });
                }
            }

            var res = _authorizationService.CreateResult(false, "Введены некорректные данные");
            return Json(res);
        }
    }
}
