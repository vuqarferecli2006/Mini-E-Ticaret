using E_Biznes.Application.Abstract.Service;
using E_Biznes.Application.DTOs.RoleDtos;
using E_Biznes.Application.Shared;
using E_Biznes.Application.Shared.Helpers;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace E_Biznes.Persistance.Services;

public class RoleService : IRoleService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly RoleCreationHelper _roleCreationHelper;
    private readonly RoleUpdateHelper _roleUpdateHelper;

    public RoleService(RoleManager<IdentityRole> roleManager, 
                        RoleCreationHelper roleCreationHelper,
                        RoleUpdateHelper roleUpdateHelper)
    {
        _roleManager = roleManager;
        _roleCreationHelper = roleCreationHelper;
        _roleUpdateHelper = roleUpdateHelper;
    }

    public async Task<BaseResponse<string?>> CreateRoleAsync(RoleCreateDto dto)
    {
        var existingRoleResponse = await _roleCreationHelper.CheckIfRoleExists(dto.Name);
        if (existingRoleResponse is not null)
            return existingRoleResponse;

        var identityRole = new IdentityRole(dto.Name);
        var createRoleResult = await _roleManager.CreateAsync(identityRole);

        if (!createRoleResult.Succeeded)
            return _roleCreationHelper.GenerateErrorResponse(createRoleResult.Errors);

        var permissionResult = await _roleCreationHelper.AddPermissionsToRole(identityRole, dto.Permissions);
        if (permissionResult is not null)
            return permissionResult;

        return new BaseResponse<string?>("Role created successfully", true, HttpStatusCode.OK);
    }


    public async Task<BaseResponse<string?>> UpdateRoleAsync(string roleId, RoleCreateDto dto)
    {
        var existRole = await _roleManager.FindByIdAsync(roleId);
        if (existRole is null)
        {
            return new("Role not found", false, HttpStatusCode.NotFound);
        }

        var permissionValidation = _roleUpdateHelper.CheckPermissionsValidity(dto.Permissions);
        if (permissionValidation is not null)
            return permissionValidation;

        var updateNameResult = await _roleUpdateHelper.UpdateRoleName(existRole, dto.Name);
        if (updateNameResult is not null)
            return updateNameResult;

        var replacePermissionsResult = await _roleUpdateHelper.ReplacePermissions(existRole, dto.Permissions);
        if (replacePermissionsResult is not null)
            return replacePermissionsResult;

        return new BaseResponse<string?>("Role updated successfully", true, HttpStatusCode.OK);
    }

}
