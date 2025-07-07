using AutoMapper;
using E_Biznes.Application.Abstract.Service;
using E_Biznes.Application.DTOs.AccountsDto;
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
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IEmailService _mailservice;

    public AccountService(UserManager<AppUser> userManager,IMapper mapper,IEmailService emailService)
    {
        _userManager = userManager;
        _mapper = mapper;
       _mailservice = emailService;
    }

    public async Task<BaseResponse<string>> AdminRegisterAsync(AccountRegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);

        if (existingUser is not null)
        {
            return new("This email is already registered", HttpStatusCode.BadRequest);
        }

        if (dto.RoleId != AccountRole.Seler && dto.RoleId != AccountRole.Moderator && dto.RoleId != AccountRole.Admin)
        {
            return new("Invalid role", HttpStatusCode.BadRequest);
        }

        AppUser user = _mapper.Map<AppUser>(dto);

        IdentityResult ıdentityResult = await _userManager.CreateAsync(user, dto.Password);

        if (!ıdentityResult.Succeeded)
        {
            StringBuilder errorMessage = new();
            foreach (var error in ıdentityResult.Errors)
            {
                errorMessage.AppendLine(error.Description);
            }
            return new(errorMessage.ToString(), HttpStatusCode.BadRequest);
        }

        string roleName = dto.RoleId switch
        {
            AccountRole.Seler => "Seller",
            AccountRole.Moderator => "Moderator",
            AccountRole.Admin => "Admin",
            _ => throw new ArgumentOutOfRangeException(nameof(dto.RoleId), "Invalid user role")
        };

        if (roleName is null)
        {
            return new("Role assignment failed", HttpStatusCode.InternalServerError);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, roleName);
        if (!roleResult.Succeeded)
        {
            return new("User created but role assignment failed", HttpStatusCode.InternalServerError);
        }

        var emailConfirmLink = await GetEmailConfirm(user);

        await _mailservice.SendEmailAsync(new List<string> { user.Email }, "Email Confirmation", emailConfirmLink);

        return new("User registered successfully", true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<UserGetDto>> GetByIdAsync(AccountGetDto dto)
    {
        var user = await _userManager.Users
            .Include(u => u.Orders) // Orders navigation property varsa
            .FirstOrDefaultAsync(u => u.Id == dto.Id);
        if (user is null)
        {
            return new("User not found", false, HttpStatusCode.NotFound);
        }
        var userDto = _mapper.Map<UserGetDto>(user);
        var roles = await _userManager.GetRolesAsync(user);
        userDto.Roles = roles.ToList();
        return new BaseResponse<UserGetDto>(userDto, true, HttpStatusCode.OK);
    }

    private async Task<string> GetEmailConfirm(AppUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var emailConfirmLink = $"https://localhost:7045/api/User/ConfirmEmail?token={HttpUtility.UrlEncode(token)}&userId={user.Id}";
        return emailConfirmLink;
    }
}
