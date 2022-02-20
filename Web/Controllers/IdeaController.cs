using ApplicationCore.Entities.IdeaEntity;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models.IdeaViewModel;
using WebUi.Controllers.Extensions;

namespace Web.Controllers
{
    public class IdeaController : ExtendedController
    {
        private readonly IIdeaRepository _ideaRepository;
        private readonly IGlobalService<Idea> _globalService;
        private readonly ITagService _tagService;

        public IdeaController(
            IIdeaRepository ideaRepository,
            IGlobalService<Idea> globalService,
            ITagService tagService)
        {
            _ideaRepository = ideaRepository;
            _globalService = globalService;
            _tagService = tagService;
        }

        [HttpGet]
        [Route("/idea/{guid}")]
        public async Task<IActionResult> Index(string? guid, int? page, string? section)
        {
            string[] allSections = new string[]
            {
                "about",
                "goals",
            };

            string validSection = allSections.Any(x => x.Equals(section)) == true 
                ? section : "about";
            

            IdeaAboutViewModel indexVm = new()
            {
                Idea = await _ideaRepository.GetIdeaDetailOrNullAsync(GetUserIdOrNull(), guid),
                TopicList = _ideaRepository.GetIdeaTopicList(guid, page),
                RecommendIdeas = await _ideaRepository.GetSimilarOrTrendsIdeasAsync(guid),
                Tags = await _tagService.GetAllTagsAsync(),
            };


            return View(indexVm);
        }
    }
}
