using E_Biznes.Application.DTOs.AccountsDto;
using E_Biznes.Application.Shared;

namespace E_Biznes.Application.Abstract.Service;

public interface IAccountService
{
    Task<BaseResponse<string>> AdminRegisterAsync(AccountRegisterDto dto);
}
