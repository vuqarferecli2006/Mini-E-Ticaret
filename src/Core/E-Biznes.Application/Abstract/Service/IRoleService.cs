using E_Biznes.Application.DTOs.RoleDtos;
using E_Biznes.Application.Shared;

namespace E_Biznes.Application.Abstract.Service;

public interface IRoleService
{
    Task<BaseResponse<string?>> CreateRoleAsync(RoleCreateDto dto);

    Task<BaseResponse<string>> DeleteRoleAsync(string id);

    Task<BaseResponse<string?>> UpdateRoleAsync(string roleId, RoleCreateDto dto);

    Task<BaseResponse<string>> DeletePermissionsAsync(string roleId, List<string> permissions);

    Task<BaseResponse<List<RoleWithPermissionsDto>>> GetRolePermissionsAsync();

    Task<BaseResponse<string>> AddPermission(string roleId,List<string> permission);
}
