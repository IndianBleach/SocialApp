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
        [Route("/admin/create/username=AlabamaUser$928&password=alabamaPassword$928")]
        public async Task<JsonResult> CreateAdmin()
        {
            var res = await _authorizationService.CreateAdminUserAsync();

            return Json(res);
        }

        public async Task<JsonResult> CleanGoalTasks()
        {
            string curUserId = GetUserIdOrNull();
            if (curUserId != null)
            {
                var res = await _adminRepisitory.CleanGoalTasksAsync(curUserId);

                return Json(res);
            }
            return Json("Not authorize for admin role or something failed");
        }

        public async Task<JsonResult> CleanTopicComments()
        {
            string curUserId = GetUserIdOrNull();
            if (curUserId != null)
            {
                var res = await _adminRepisitory.CleanTopicCommentsAsync(curUserId);

                return Json(res);
            }
            return Json("Not authorize for admin role or something failed");
        }

        public async Task<JsonResult> RemoveTopic(string topicId)
        {
            string curUserId = GetUserIdOrNull();
            if (curUserId != null)
            {
                var res = await _adminRepisitory.RemoveTopicByIdAsync(topicId, curUserId);

                return Json(res);
            }
            return Json("Not authorize for admin role or something failed");
        }

        public async Task<JsonResult> RemoveGoal(string goalId)
        {
            string curUserId = GetUserIdOrNull();
            if (curUserId != null)
            {
                var res = await _adminRepisitory.RemoveGoalByIdAsync(goalId, curUserId);

                return Json(res);
            }
            return Json("Not authorize for admin role or something failed");
        }

        public async Task<JsonResult> RemoveUserById(string userId)
        {
            string curUserId = GetUserIdOrNull();
            if (curUserId != null)
            {
                var res = await _adminRepisitory.RemoveAccountByIdAsync(userId, curUserId);

                return Json(res);
            }
            return Json("Not authorize for admin role or something failed");
        }

        public async Task<JsonResult> RemoveIdeaById(string ideaId)
        {
            string curUserId = GetUserIdOrNull();
            if (curUserId != null)
            {
                var res = await _adminRepisitory.RemoveIdeaByIdAsync(ideaId, curUserId);

                return Json(res);
            }
            return Json("Not authorize for admin role or something failed");
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
