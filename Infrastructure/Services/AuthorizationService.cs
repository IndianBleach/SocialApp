using ApplicationCore.DTOs.Authorization;
using ApplicationCore.Identity;
using ApplicationCore.Interfaces;
using AutoMapper;
using Infrastructure.Constants;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationContext _dbContext;
        private readonly ITagService _tagService;

        public AuthorizationService(
            ApplicationContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITagService tagService)
        {
            _dbContext = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _tagService = tagService;
        }

        public AuthorizationResultDto CreateResult(bool success, string message)
            => new(success, message);

        public async Task<AuthorizationResultDto> UserSignInAsync(UserSignInDto model)
        {
            var findUser = await _userManager.FindByNameAsync(model.Username);

            var isValidPassword = await _userManager.CheckPasswordAsync(findUser, model.Password);

            if (isValidPassword)
            {                
                await _signInManager.SignInAsync(findUser, false);

                return CreateResult(true, "Вход в аккаунт выполнен");
            }
            else return CreateResult(false, "Неправильные данные при вводе");
        }

        public async Task UserSignOutAsync()
            => await _signInManager.SignOutAsync();

        public async Task<AuthorizationResultDto> UserSignUpAsync(UserSignUpDto model)
        {
            var config = new MapperConfiguration(conf => conf.CreateMap<UserSignUpDto, ApplicationUser>()
            .ForMember("UserName", opt => opt.MapFrom(x => x.Username))
            .ForMember(x => x.Tags, opt => opt.MapFrom(x => _tagService.CreateTagList(x.Tags)))
            .ForMember("Avatar", opt => opt.MapFrom(x => _dbContext.UserAvatars
                .FirstOrDefault(x => x.Name.Equals(AvatarInformation.UserDefaultAvatarName)))));

            var mapper = new Mapper(config);

            ApplicationUser createUser = mapper.Map<ApplicationUser>(model);

            var result = await _userManager.CreateAsync(createUser, model.Password);

            Claim avatarClaim = new("UserAvatarName", AvatarInformation.UserDefaultAvatarName);
            Claim guidClaim = new("UserId", createUser.Id);   

            if (result.Succeeded)
            {
                await _userManager.AddClaimAsync(createUser, guidClaim);
                await _userManager.AddClaimAsync(createUser, avatarClaim);
                await _userManager.AddToRoleAsync(createUser, "user");
                await _signInManager.SignInAsync(createUser, false);

                return CreateResult(true, "Регистрация прошла успешно!");
            }

            else return CreateResult(false, "При регистрации что-то пошло не так, попробуйте снова");
        }
    }
}
