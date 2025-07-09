using AutoMapper;
using E_Biznes.Application.Abstract.Service;
using E_Biznes.Application.DTOs.AccountsDto;
using E_Biznes.Application.DTOs.OrderDtos;
using E_Biznes.Application.DTOs.UserDtos;
using E_Biznes.Application.Shared;
using E_Biznes.Domain.Entities;
using E_Biznes.Domain.Enum;
using E_Biznes.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;
using System.Web;

namespace E_Biznes.Persistance.Services;

public class AccountService : IAccountService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IEmailService _mailservice;

    public AccountService(UserManager<AppUser> userManager, IMapper mapper, IEmailService emailService, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _mapper = mapper;
        _mailservice = emailService;
        _roleManager = roleManager;
    }

    public async Task<BaseResponse<string>> RegisterAdminAccountAsync(AccountRegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);//dto-dan gelen emaili yoxlayir
        if (existingUser is not null)
            return new("This email is already registered", HttpStatusCode.BadRequest);

        if (!Enum.IsDefined(typeof(PlatformRole), dto.RoleId))//girilen reqeme uygun role yoxlayir
            return new("Invalid role", HttpStatusCode.BadRequest);

        AppUser user = _mapper.Map<AppUser>(dto);
        IdentityResult ıdentityResult = await _userManager.CreateAsync(user, dto.Password);// yeni user yaratmaq ucun passwordu ile beraber
        if (!ıdentityResult.Succeeded)
        {
            StringBuilder errorMessage = new();
            foreach (var error in ıdentityResult.Errors)
            {
                errorMessage.AppendLine(error.Description);
            }
            return new(errorMessage.ToString(), HttpStatusCode.BadRequest);
        }
        string roleName = dto.RoleId switch// roleId-nin dəyərinə görə uyğun role adını təyin edir
        {
            PlatformRole.Seller => "Seller",
            PlatformRole.Moderator => "Moderator",
            PlatformRole.Admin => "Admin",
            _ => throw new ArgumentOutOfRangeException(nameof(dto.RoleId), "Invalid user role")
        };

        if (roleName is null)
            return new("Role assignment failed", HttpStatusCode.InternalServerError);

        var roleResult = await _userManager.AddToRoleAsync(user, roleName);
        if (!roleResult.Succeeded)
            return new("User created but role assignment failed", HttpStatusCode.InternalServerError);

        var emailConfirmLink = await GenerateEmailConfirmationLinkAsync(user);
        await _mailservice.SendEmailAsync(new List<string> { user.Email }, "Email Confirmation", emailConfirmLink);

        return new("User registered successfully", true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<UserGetDto>> GetByIdAsync(AccountGetDto dto)
    {
        var user = await _userManager.Users
            .Include(u => u.Orders) // Orders navigation property varsa
            .FirstOrDefaultAsync(u => u.Id == dto.Id);
        if (user is null)
            return new("User not found", false, HttpStatusCode.NotFound);

        var userDto = _mapper.Map<UserGetDto>(user);
        var roles = await _userManager.GetRolesAsync(user);
        userDto.Roles = roles.ToList();

        return new BaseResponse<UserGetDto>(userDto, true, HttpStatusCode.OK);
    }

    private async Task<string> GenerateEmailConfirmationLinkAsync(AppUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var emailConfirmLink = $"https://localhost:7045/api/User/ConfirmEmail?token={HttpUtility.UrlEncode(token)}&userId={user.Id}";
        return emailConfirmLink;
    }

    public async Task<BaseResponse<string>> AddRole(UserAddRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
        if (user is null)
        {
            return new BaseResponse<string>("User not found", false, HttpStatusCode.NotFound);
        }
        var seenRoleNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var roleNamesToAssign = new List<string>();
        foreach (var roleId in dto.RoleId.Distinct())
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());

            if (role == null || string.IsNullOrWhiteSpace(role.Name))
            {
                return new BaseResponse<string>($"Role with ID '{roleId}' is invalid or has no name", false, HttpStatusCode.BadRequest);
            }
            if (await _userManager.IsInRoleAsync(user, role.Name))
            {
                return new BaseResponse<string>($"User already has the role '{role.Name}'", false, HttpStatusCode.BadRequest);
            }
            if (!seenRoleNames.Add(role.Name))
            {
                return new BaseResponse<string>($"Duplicate role '{role.Name}' detected in request", false, HttpStatusCode.BadRequest);
            }

            roleNamesToAssign.Add(role.Name);
        }
        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Any())
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                return new BaseResponse<string>($"Failed to remove existing roles: {errors}", false, HttpStatusCode.InternalServerError);
            }
        }
        foreach (var roleName in roleNamesToAssign)
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new BaseResponse<string>($"Failed to assign role '{roleName}': {errors}", false, HttpStatusCode.BadRequest);
            }
        }
        return new BaseResponse<string>(
            $"Roles updated successfully: {string.Join(", ", roleNamesToAssign)}",
            true,
            HttpStatusCode.OK
        );
    }

    public async Task<BaseResponse<List<UserGetDto>>> GetAllAsync()
    {
        var users = await _userManager.Users
            .Include(u => u.Orders)
                .ThenInclude(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
            .ToListAsync();
        if (users.Count == 0)
        {
            return new("No users found", false, HttpStatusCode.NotFound);
        }
        var userDtos = users.Select(user =>
        {
            var dto = _mapper.Map<UserGetDto>(user);
            var roles = _userManager.GetRolesAsync(user).Result;
            dto.Roles = roles.ToList();
            dto.Orders = _mapper.Map<List<OrderGetDto>>(user.Orders);
            return dto;
        }).ToList();
        return new(userDtos, true, HttpStatusCode.OK);
    }

}
