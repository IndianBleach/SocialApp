﻿using ApplicationCore.DTOs.Idea;
using ApplicationCore.DTOs.Tag;
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
                "editGeneral"
            };

            string validSection = allSections.Any(x => x.Equals(section)) == true 
                ? section : "about";

            if (validSection.Equals("goals"))
            {
                IdeaGoalsViewModel goalsVm = new()
                {
                    Idea = await _ideaRepository.GetIdeaDetailOrNullAsync(GetUserIdOrNull(), guid),
                    GoalList = _ideaRepository.GetIdeaGoalList(guid, page),
                    RecommendIdeas = await _ideaRepository.GetSimilarOrTrendsIdeasAsync(guid),
                    Tags = await _tagService.GetAllTagsAsync(),
                };

                return View("Goals", goalsVm);
            }
            else if (validSection.Equals("editGeneral"))
            {
                IdeaDetailDto idea = await _ideaRepository.GetIdeaDetailOrNullAsync(GetUserIdOrNull(), guid);
                List<TagDto> allTags = await _tagService.GetAllTagsAsync();
                IdeaEditGeneralViewModel editVm = new()
                {
                    Idea = idea,
                    RecommendIdeas = await _ideaRepository.GetSimilarOrTrendsIdeasAsync(guid),
                    Tags = allTags,
                    EditTags = _tagService.GroupSelfAndOtherTags(allTags, idea.Tags.ToList()),
                    EditStatuses = await _ideaRepository.GetAllIdeaStatusesAsync(),
                    EditDescription = await _ideaRepository.GetIdeaDescriptionAsync(guid)
                };

                return View("EditGeneral", editVm);
            }



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
