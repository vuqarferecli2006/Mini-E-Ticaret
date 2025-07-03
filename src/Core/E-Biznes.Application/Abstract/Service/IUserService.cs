using E_Biznes.Application.DTOs.UserDtos;
using E_Biznes.Application.Shared;

namespace E_Biznes.Application.Abstract.Service;

public interface IUserService
{
    Task<BaseResponse<string>> RegisterAsync(UserRegisterDto dto);

    Task<BaseResponse<TokenResponse>> LoginAsync(UserLoginDto dto);
}
