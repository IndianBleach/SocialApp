using ApplicationCore.DTOs;
using ApplicationCore.DTOs.Tag;
using ApplicationCore.DTOs.User;
using System.Collections.Generic;

namespace WebUi.Models
{
    public class FindUsersViewModelUpdated
    {        
        public ICollection<HomeNewsDto> LastNews { get; set; }
        public ICollection<RecommendUserDto>? RecommendUsers { get; set; }
        public UserList UserList { get; set; }
        public List<TagDto> Tags { get; set; }
        public bool IsAuthorized { get; set; }
        public string? Search { get; set; }
        public string? SearchTag { get; set; }
        public string? SearchCountry { get; set; }
        public string? SearchCity { get; set; }
        public string? SearchParamsResultString { get; set; }

        public FindUsersViewModelUpdated(
             ICollection<HomeNewsDto> news,
             ICollection<RecommendUserDto>? recommends,
             UserList users,
             List<TagDto> tags,
             bool isAuthorized,
             string? search,
             string? tag,
             string? country,
             string? city,
             string? paramResult
             )
        {
            LastNews = news;
            RecommendUsers = recommends;
            UserList = users;
            Tags = tags;
            IsAuthorized = isAuthorized;
            Search = search;
            SearchTag = tag;
            SearchCountry = country;
            SearchCity = city;
            SearchParamsResultString = paramResult;
        }
    }

    public class FindUsersViewModel
    {
        public ICollection<PageInfoDto> Pages { get; set; }
        public ICollection<HomeNewsDto> LastNews { get; set; }
        public ICollection<RecommendUserDto>? RecommendUsers { get; set; }
        public ICollection<UserDto> Users { get; set; }
        public List<TagDto> Tags { get; set; }
        public bool IsAuthorized { get; set; }
        public string? Search { get; set; }
        public string? SearchTag { get; set; }
        public string? SearchCountry { get; set; }
        public string? SearchCity { get; set; }
        public string? SearchParamsResultString { get; set; }
    }
}
