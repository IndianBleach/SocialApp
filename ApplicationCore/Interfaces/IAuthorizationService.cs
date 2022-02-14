using ApplicationCore.DTOs.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IAuthorizationService
    {
        AuthorizationResultDto CreateResult(bool success, string message);
        Task<AuthorizationResultDto> UserSignUpAsync(UserSignUpDto model);
        Task<AuthorizationResultDto> UserSignInAsync(UserSignInDto model);
        Task UserSignOutAsync();
    }
}
