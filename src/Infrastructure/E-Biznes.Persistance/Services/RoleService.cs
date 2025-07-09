using E_Biznes.Application.Abstract.Service;
using E_Biznes.Application.DTOs.RoleDtos;
using E_Biznes.Application.Shared;
using E_Biznes.Application.Shared.Helpers;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Security.Claims;

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

    public async Task<BaseResponse<string>> AddPermission(string roleId, List<string> permissions)
    {
        if (string.IsNullOrWhiteSpace(roleId))
            return new("Id mustn't be empty", HttpStatusCode.BadRequest);
        if(permissions is null || !permissions.Any())
            return new("Permissions mustn't be empty", HttpStatusCode.BadRequest);

        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null)
            return new("Role not found", false, HttpStatusCode.NotFound);

        var permissionList = PermissionHelper.GetPermissionList();
        var invalidPermissions = permissions
            .Where(p => !permissionList.Contains(p, StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (invalidPermissions.Any())
        {
            var invalids = string.Join(", ", invalidPermissions);
            return new($"Invalid permissions: {invalids}", false, HttpStatusCode.BadRequest);
        }

        var existingClaims = await _roleManager.GetClaimsAsync(role);
        var existingPermissionValues = existingClaims
            .Where(c => c.Type == "Permission")
            .Select(c => c.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var newPermissions = permissions
            .Where(p => !existingPermissionValues.Contains(p))
            .ToList();

        foreach (var perm in newPermissions)
        {
            var result = await _roleManager.AddClaimAsync(role, new Claim("Permission", perm));
            if (!result.Succeeded)
            {
                var error = string.Join("; ", result.Errors.Select(e => e.Description));
                return new($"Failed to add permission '{perm}': {error}", false, HttpStatusCode.BadRequest);
            }
        }

        return new("Permissions added successfully", true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string?>> UpdateRoleAsync(string roleId, RoleCreateDto dto)
    {
        if (roleId is null)
            return new("Id mustn't be empty", HttpStatusCode.BadRequest);

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

    public async Task<BaseResponse<string>> DeleteRoleAsync(string id)
    {
        if (id is null)
            return new("Id mustn't be empty", HttpStatusCode.BadRequest);

        var role = await _roleManager.FindByIdAsync(id);
        if (role is null)
        {
            return new BaseResponse<string>("Role not found", false, HttpStatusCode.NotFound);
        }

        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new BaseResponse<string>($"Failed to delete role: {errors}", false, HttpStatusCode.BadRequest);
        }

        return new BaseResponse<string>("Role deleted successfully", true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> DeletePermissionsAsync(string roleId, List<string> permissions)
    {
        if (roleId is null)
            return new("Id mustn't be empty", HttpStatusCode.BadRequest);

        if (permissions is null || !permissions.Any())
            return new("Permissions mustn't be empty", HttpStatusCode.BadRequest);

        var role = await _roleManager.FindByIdAsync(roleId);
        if (role is null)
            return new("Role not found", false, HttpStatusCode.NotFound);

        var existingClaims = await _roleManager.GetClaimsAsync(role);
        var existingPermissionClaims = existingClaims.Where(c => c.Type == "Permission").ToList();

        // Yoxla ki, bütün daxil olan permission-lar rolda var
        var notFoundPermissions = permissions
            .Where(p => !existingPermissionClaims.Any(ec => ec.Value == p))
            .ToList();

        if (notFoundPermissions.Any())
        {
            var joined = string.Join(", ", notFoundPermissions);
            return new($"The following permissions were not found on this role: {joined}", false, HttpStatusCode.BadRequest);
        }

        // Əgər hamısı varsa - sil
        foreach (var permission in permissions)
        {
            var claimToRemove = existingPermissionClaims.First(c => c.Value == permission);
            var result = await _roleManager.RemoveClaimAsync(role, claimToRemove);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new($"Failed to remove permission '{permission}': {errors}", false, HttpStatusCode.BadRequest);
            }
        }

        return new("Selected permissions removed successfully", true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<List<RoleWithPermissionsDto>>> GetRolePermissionsAsync()
    {
        var roles = _roleManager.Roles.ToList();

        var result = new List<RoleWithPermissionsDto>();

        foreach (var role in roles)
        {
            var claims = await _roleManager.GetClaimsAsync(role);
            var permissions = claims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
                .ToList();

            result.Add(new RoleWithPermissionsDto
            {
                RoleId = role.Id,
                RoleName = role.Name,
                Permissions = permissions
            });
        }

        return new BaseResponse<List<RoleWithPermissionsDto>>(result, true, HttpStatusCode.OK);
    }

}
