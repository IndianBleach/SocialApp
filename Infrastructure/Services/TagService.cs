using ApplicationCore.DTOs.AsyncLoad.Idea;
using ApplicationCore.DTOs.Tag;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class TagService : ITagService
    {
        private readonly ApplicationContext _dbContext;

        public TagService(ApplicationContext context)
        {
            _dbContext = context;
        }

        public List<Tag> CreateTagList(ICollection<string> choicedTags)
        {
            List<Tag> tags = new List<Tag>();
            if (choicedTags != null)
                foreach (string tagGuid in choicedTags)
                {
                    Tag getTag = _dbContext.Tags.FirstOrDefault(x => x.Id == tagGuid);
                    if (getTag != null)
                        tags.Add(getTag);
                }

            return tags;
        }

        public async Task<List<Tag>> CreateTagListAsync(ICollection<string> choicedTags)
        {
            List<Tag> tags = new List<Tag>();
            if (choicedTags != null)
                foreach (string tagGuid in choicedTags)
                {
                    Tag getTag = await _dbContext.Tags.FirstOrDefaultAsync(x => x.Id == tagGuid);                
                    if (getTag != null)
                        tags.Add(getTag);
                }
                    
            return tags;
        }

        public async Task<List<TagDto>> GetAllTagsAsync()
        {
            return await _dbContext.Tags
                .Select(x => new TagDto()
                {
                    Guid = x.Id,
                    Name = x.Name
                }).ToListAsync();
        }

        public IEnumerable<EditTagDto> GroupSelfAndOtherTags(ICollection<TagDto> allTags, ICollection<TagDto> sourceTags)
        {
            return allTags.Select(x =>
            new EditTagDto()
            {
                Guid = x.Guid,
                Name = x.Name,
                Selected = sourceTags.Any(e => e.Guid.Equals(x.Guid))
            });            
        }
    }
}
