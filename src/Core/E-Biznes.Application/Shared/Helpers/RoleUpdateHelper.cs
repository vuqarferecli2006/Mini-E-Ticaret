using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Security.Claims;

namespace E_Biznes.Application.Shared.Helpers;

public class RoleUpdateHelper
{
    private readonly RoleManager<IdentityRole> _roleManager;
    public RoleUpdateHelper(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }
    public BaseResponse<string?>? CheckPermissionsValidity(IEnumerable<string> permissions)
    {
        foreach (var permission in permissions.Distinct())
        {
            if (!PermissionHelper.GetPermissionList().Contains(permission))
            {
                return new($"Permission '{permission}' does not exist", false, HttpStatusCode.BadRequest);
            }
        }
        return null;
    }

    public async Task<BaseResponse<string?>?> UpdateRoleName(IdentityRole role, string newName)
    {
        role.Name = newName;
        var updateResult = await _roleManager.UpdateAsync(role);

        if (!updateResult.Succeeded)
        {
            var errorMessages = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            return new BaseResponse<string?>($"Failed to update role name: {errorMessages}", false, HttpStatusCode.BadRequest);
        }

        return null;
    }

    public async Task<BaseResponse<string?>?> ReplacePermissions(IdentityRole role, IEnumerable<string> permissions)
    {
        var currentClaims = await _roleManager.GetClaimsAsync(role);

        foreach (var claim in currentClaims.Where(c => c.Type == "Permission"))
        {
            await _roleManager.RemoveClaimAsync(role, claim);
        }

        foreach (var permission in permissions.Distinct())
        {
            var addClaimResult = await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            if (!addClaimResult.Succeeded)
            {
                var errors = string.Join(", ", addClaimResult.Errors.Select(e => e.Description));
                return new BaseResponse<string?>($"Failed to add permission '{permission}': {errors}", false, HttpStatusCode.BadRequest);
            }
        }

        return null;
    }

}
