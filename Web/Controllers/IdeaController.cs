using ApplicationCore.DTOs.Idea;
using ApplicationCore.DTOs.Tag;
using ApplicationCore.Entities.IdeaEntity;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models.IdeaViewModel;
using WebUi.Controllers.Extensions;

namespace Web.Controllers
{
    [Authorize]
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
                "editgeneral",
                "editmodders",
                "editmembers",
                "editremove"
            };

            string validSection = allSections.Any(x => x.Equals(section?.ToLower())) == true 
                ? section?.ToLower() : "about";

            IdeaDetailDto idea = await _ideaRepository.GetIdeaDetailOrNullAsync(GetUserIdOrNull(), guid);
            List<TagDto> allTags = await _tagService.GetAllTagsAsync();

            bool isAuthor = idea.CurrentRole.Role.Equals(CurrentUserRoleTypes.Author);

            if (validSection.Equals("goals"))
            {
                IdeaGoalsViewModel goalsVm = new()
                {
                    Idea = idea,
                    GoalList = _ideaRepository.GetIdeaGoalList(guid, page),
                    RecommendIdeas = await _ideaRepository.GetSimilarOrTrendsIdeasAsync(guid),
                    Tags = allTags,
                };

                return View("Goals", goalsVm);
            }
            else if (validSection.Equals("editgeneral") && isAuthor)
            {
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
            else if (validSection.Equals("editmodders") && isAuthor)
            {
                IdeaEditMembersViewModel editVm = new()
                {
                    Idea = idea,
                    RecommendIdeas = await _ideaRepository.GetSimilarOrTrendsIdeasAsync(guid),
                    Tags = allTags,
                    MemberList = await _ideaRepository.GetIdeaMemberListByRoleOrNull(guid, IdeaMemberRoles.Modder, page)
                };

                return View("EditModders", editVm);
            }
            else if (validSection.Equals("editmembers") && isAuthor)
            {
                IdeaEditMembersViewModel editVm = new()
                {
                    Idea = idea,
                    RecommendIdeas = await _ideaRepository.GetSimilarOrTrendsIdeasAsync(guid),
                    Tags = allTags,
                    MemberList = await _ideaRepository.GetIdeaMemberListByRoleOrNull(guid, IdeaMemberRoles.Member, page)
                };

                return View("EditMembers", editVm);
            }
            else if (validSection.Equals("editremove") && isAuthor)
            {
                IdeaEditRemoveViewModel editVm = new()
                {
                    Idea = idea,
                    RecommendIdeas = await _ideaRepository.GetSimilarOrTrendsIdeasAsync(guid),
                    Tags = allTags,
                };

                return View("EditRemove", editVm);
            }
            else
            {
                IdeaAboutViewModel indexVm = new()
                {
                    Idea = idea,
                    TopicList = _ideaRepository.GetIdeaTopicList(guid, page),
                    RecommendIdeas = await _ideaRepository.GetSimilarOrTrendsIdeasAsync(guid),
                    Tags = allTags,
                };
                return View(indexVm);
            }                        
        }
    }
}
