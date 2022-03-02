using ApplicationCore.DTOs.Create;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebUi.Controllers.Extensions;

namespace WebUi.Controllers
{
    [Authorize]
    public class CreateController : ExtendedController
    {
        private readonly IIdeaRepository _ideaRepository;
        public CreateController(IIdeaRepository ideaRepository)
        {
            _ideaRepository = ideaRepository;
        }

        
        [HttpPost]
        [Route("/create/idea")]
        public async Task<JsonResult> Idea(CreateIdeaDto model)
        {
            model.AuthorGuid = GetUserIdOrNull();

            if (ModelState.IsValid)
            {
                var res = await _ideaRepository.CreateIdeaAsync(model);
                return Json(res);
            }

            return Json(null);
        }
    }
}
