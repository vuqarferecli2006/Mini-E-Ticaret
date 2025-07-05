using AzBinaTeam.Application.DTOs.UserDtos;
using E_Biznes.Application.DTOs.UserDtos;
using E_Biznes.Application.Shared;

namespace E_Biznes.Application.Abstract.Service;

public interface IUserService
{
    Task<BaseResponse<string>> RegisterAsync(UserRegisterDto dto);

    Task<BaseResponse<TokenResponse>> LoginAsync(UserLoginDto dto);

    Task<BaseResponse<string>> AddRole(UserAddRoleDto dto);

    Task<BaseResponse<TokenResponse>> RefreshTokenAsync(RefrexhTokenRequestDto request);

    Task<BaseResponse<string>> ConfirmEmail(string userId, string token);

    Task<BaseResponse<string>> ResetPassword(UserResetPasswordDto dto);

    Task<BaseResponse<string>> SendResetPasswordEmail(string email);
}
