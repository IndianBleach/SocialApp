using ApplicationCore.DTOs.Authorization;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebUi.Controllers.Extensions;
using WebUi.Models;

namespace Web.Controllers
{
    public class AuthorizeController : ExtendedController
    {
        private readonly ApplicationCore.Interfaces.IAuthorizationService _authorizationService;
        private readonly ITagService _tagService;

        public AuthorizeController(ApplicationCore.Interfaces.IAuthorizationService authService,
            ITagService tagService)
        {
            _authorizationService = authService;
            _tagService = tagService;
        }


        [Route("/authorize/signout")]
        public async Task<IActionResult> SignOut()
        {
            await _authorizationService.UserSignOutAsync();

            return RedirectToAction("index", "authorize");
        }


        [HttpPost]
        [Route("/authorize/signin")]
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

        [HttpPost]
        [Route("/authorize/signup")]
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

        [HttpGet]
        [Route("/")]
        [Route("/account/login")]
        public async Task<IActionResult> Index()
        {
            if (IsUserAuthenticated())
                return RedirectToAction("index", "home");

            return View(new DemoViewModel(await _tagService.GetAllTagsAsync()));
        }
    }
}
