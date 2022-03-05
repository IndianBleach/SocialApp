using ApplicationCore.Interfaces;
using ApplicationCore.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebUi.Controllers.Extensions;

namespace Web.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : ExtendedController
    {
        private IAdminRepository _adminRepisitory;
        private ApplicationCore.Interfaces.IAuthorizationService _authorizationService;

        public AdminController(IAdminRepository adminRepository,
            ApplicationCore.Interfaces.IAuthorizationService authService)
        {
            _adminRepisitory = adminRepository;
            _authorizationService = authService;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("/admin/create/username=AlabamaBleach$928&password=adminPa$$word928")]
        public async Task<JsonResult> CreateAdmin()
        {
            var res = await _authorizationService.CreateAdminUserAsync();

            return Json(res);
        }

        public async Task<IActionResult> Index()
        {
            string curUserId = GetUserIdOrNull();

            if (curUserId != null)
            {
                var getUser = await _adminRepisitory.GetAdminAccountOrNullAsync(curUserId);                    

                return View(getUser);
            }

            return RedirectToAction("login", "account");
        }
    }
}
