using E_Biznes.Application.DTOs.AccountsDto;
using E_Biznes.Application.DTOs.UserDtos;
using E_Biznes.Application.Shared;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace E_Biznes.Application.Abstract.Service;

public interface IAccountService
{
    Task<BaseResponse<string>> AdminRegisterAsync(AccountRegisterDto dto);

    Task<BaseResponse<UserGetDto>> GetByIdAsync(AccountGetDto dto);
}
