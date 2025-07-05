using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Security.Claims;

namespace E_Biznes.Application.Shared.Helpers;

public class RoleCreationHelper
{

    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleCreationHelper(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<BaseResponse<string?>?> CheckIfRoleExists(string roleName)
    {
        var existingRole = await _roleManager.FindByNameAsync(roleName);
        if (existingRole is not null)
            return new("Role already exists", false, HttpStatusCode.NotFound);

        return null;
    }

    public BaseResponse<string?> GenerateErrorResponse(IEnumerable<IdentityError> errors)
    {
        var errorMessage = string.Join(", ", errors.Select(e => e.Description));
        return new(errorMessage, HttpStatusCode.BadRequest);
    }

    public async Task<BaseResponse<string?>?> AddPermissionsToRole(IdentityRole role, IEnumerable<string> permissions)
    {
        foreach (var permission in permissions.Distinct())
        {
            if (!PermissionHelper.GetPermissionList().Contains(permission))
            {
                await _roleManager.DeleteAsync(role);
                return new($"Permission '{permission}' does not exist", HttpStatusCode.BadRequest);
            }

            var claimResult = await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            if (!claimResult.Succeeded)
            {
                var errorMessage = string.Join("; ", claimResult.Errors.Select(e => e.Description));
                return new($"An error occurred while adding permission '{permission}': {errorMessage}", HttpStatusCode.BadRequest);
            }
        }

        return null;
    }

}
