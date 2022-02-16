using ApplicationCore.DTOs;
using ApplicationCore.DTOs.Idea;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IGlobalService<T> where T : class, IBaseEntity
    {
        string? CreateSearchResultString(string? tag, string? search, string? country, string? city);
        ICollection<PageInfoDto> CreatePages(int totalCount, int? currentPage);
        ICollection<PageInfoDto> CreatePages(int? currentPage);
        Task<ICollection<HomeNewsDto>> GetLastNewsAsync();
        Task<ICollection<SearchReactionDto>> GetSearchReactionsAsync();
    }
}
