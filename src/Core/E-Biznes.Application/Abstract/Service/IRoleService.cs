using E_Biznes.Application.DTOs.RoleDtos;
using E_Biznes.Application.Shared;

namespace E_Biznes.Application.Abstract.Service;

public interface IRoleService
{
    Task<BaseResponse<string?>> CreateRoleAsync(RoleCreateDto dto);

    Task<BaseResponse<string?>> UpdateRoleAsync(string roleId, RoleCreateDto dto);
}
