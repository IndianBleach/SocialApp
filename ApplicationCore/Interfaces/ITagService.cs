using ApplicationCore.DTOs.AsyncLoad.Idea;
using ApplicationCore.DTOs.Tag;
using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface ITagService
    {
        IEnumerable<EditTagDto> GroupSelfAndOtherTags(ICollection<TagDto> allTags, ICollection<TagDto> sourceTags);
        List<Tag> CreateTagList(ICollection<string> tags);
        Task<List<Tag>> CreateTagListAsync(ICollection<string> tags);
        Task<List<TagDto>> GetAllTagsAsync();        
    }
}
