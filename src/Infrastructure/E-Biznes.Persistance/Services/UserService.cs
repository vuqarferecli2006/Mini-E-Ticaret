using AutoMapper;
using AzBinaTeam.Application.DTOs.UserDtos;
using E_Biznes.Application.Abstract.Repositories;
using E_Biznes.Application.Abstract.Service;
using E_Biznes.Application.DTOs.OrderDtos;
using E_Biznes.Application.DTOs.ProducDtos;
using E_Biznes.Application.DTOs.UserDtos;
using E_Biznes.Application.Shared;
using E_Biznes.Application.Shared.Settings;
using E_Biznes.Domain.Entities;
using E_Biznes.Domain.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace E_Biznes.Persistance.Services;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly JwtSetting _jwtSetting;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;
    private readonly IEmailService _mailservice;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;

    public UserService(UserManager<AppUser> userManager,
                        IMapper mapper,
                        SignInManager<AppUser> signInManager,
                        IOptions<JwtSetting> jwtSetting,
                        RoleManager<IdentityRole> roleManager,
                        IEmailService mailservice,
                        IHttpContextAccessor httpContextAccessor,
                        IProductRepository productRepository,
                        IOrderRepository orderRepository)

    {
        _userManager = userManager;
        _mapper = mapper;
        _signInManager = signInManager;
        _jwtSetting = jwtSetting.Value;
        _roleManager = roleManager;
        _mailservice = mailservice;
        _httpContextAccessor = httpContextAccessor;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
    }



    public async Task<BaseResponse<string>> RegisterAsync(UserRegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser is not null)
        {
            return new("This email is already registered", HttpStatusCode.BadRequest);
        }
        if (dto.RoleId != MarketplaceRole.Buyer && dto.RoleId != MarketplaceRole.Seller)
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
            MarketplaceRole.Buyer => "Buyer",
            MarketplaceRole.Seller => "Seller",
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

    public async Task<BaseResponse<string>> ConfirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return new("Email confirmation failed", false, HttpStatusCode.BadRequest);
        }
        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            return new("Email confirmation failed", false, HttpStatusCode.BadRequest);
        }
        return new("Email confirmed successfully", true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<TokenResponse>> LoginAsync(UserLoginDto dto)
    {
        var useremail = await _userManager.FindByEmailAsync(dto.Email);
        if (useremail is null)
        {
            return new("Email or password incorrect", HttpStatusCode.NotFound);
        }
        if (!useremail.EmailConfirmed)
        {
            return new("Please confirm your email", false, HttpStatusCode.BadRequest);
        }
        SignInResult signInResult = await _signInManager.PasswordSignInAsync(useremail, dto.Password, true, true);
        if (!signInResult.Succeeded)
        {
            return new("Email or password incorrect", HttpStatusCode.NotFound);
        }
        var token = await GenerateJwttoken(useremail);
        return new("Login successful", token,true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<TokenResponse>> RefreshTokenAsync(RefrexhTokenRequestDto request)
    {
        var principal = GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
        {
            return new BaseResponse<TokenResponse>("Invalid token", false, HttpStatusCode.Unauthorized);
        }
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) 
        {
            return new BaseResponse<TokenResponse>("User not found", false, HttpStatusCode.NotFound);
        }
        if (user.RefreshToken is null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow) 
        {
            return new BaseResponse<TokenResponse>("Invalid refresh token", false, HttpStatusCode.Unauthorized);
        }
        var tokenResponse = await GenerateJwttoken(user);
        return new("Token refreshed successfully", tokenResponse,true, HttpStatusCode.OK);
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

    public async Task<BaseResponse<string>> ResetPassword(UserResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
        {
            return new("User not found", false, HttpStatusCode.NotFound);
        }
        if (!user.EmailConfirmed)
        {
            return new("Please confirm your email", false, HttpStatusCode.BadRequest);
        }
        var decodedToken = HttpUtility.UrlDecode(dto.Token);
        var result = await _userManager.ResetPasswordAsync(user, decodedToken, dto.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new($"Failed to reset password: {errors}", false, HttpStatusCode.BadRequest);
        }
        return new("Password reset successfully", true, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> SendResetPasswordEmail(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return new BaseResponse<string>("User not found", false, HttpStatusCode.NotFound);
        }
        var resetLink = await GetEmailResetConfirm(user);
        await _mailservice.SendEmailAsync(new List<string> { user.Email }, "Reset Password", resetLink);
        return new BaseResponse<string>("Email confirmed successfully", true, HttpStatusCode.OK);
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

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false, 
            ValidIssuer = _jwtSetting.Issuer,
            ValidAudience = _jwtSetting.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.Key))
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is JwtSecurityToken jwtToken &&
                jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                return principal;
            }
        }
        catch (Exception)
        {

            return null;
        }

        return null;
    }

    private async Task<string> GetEmailResetConfirm(AppUser user)
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var link = $"https://localhost:7045/api/User/SendResetConfirmEmail?email={user.Email}&token={HttpUtility.UrlEncode(token)}";
        Console.WriteLine("Reset Password Link : " + link);
        return link;
    }

    private async Task<TokenResponse> GenerateJwttoken(AppUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSetting.Key);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email)
        };
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var roleName in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, roleName));
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role is not null)
            {
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                var rolePermissions = roleClaims
                    .Where(c => c.Type == "Permission")
                    .ToList();
                foreach (var permission in rolePermissions)
                {
                    claims.Add(new Claim("Permission", permission.Value));
                }
            }
        }
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_jwtSetting.ExpireinMinutes).AddMinutes(1);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            NotBefore = now,
            IssuedAt = now,
            Issuer = _jwtSetting.Issuer,
            Audience = _jwtSetting.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);

        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiryDate = DateTime.UtcNow.AddHours(2);
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = refreshTokenExpiryDate;
        await _userManager.UpdateAsync(user);
        return new TokenResponse
        {
            Token = jwt.Trim(),
            ExpireDate = tokenDescriptor.Expires!.Value,
            RefreshToken = refreshToken,
        };

    }

    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private async Task<string> GetEmailConfirm(AppUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var emailConfirmLink = $"https://localhost:7045/api/User/ConfirmEmail?token={HttpUtility.UrlEncode(token)}&userId={user.Id}";
        return emailConfirmLink;
    }

    public async Task<BaseResponse<UserProfileDto>> GetCurrentUserProfileAsync()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return new BaseResponse<UserProfileDto>("Unauthorized", false, HttpStatusCode.Unauthorized);
        }
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new BaseResponse<UserProfileDto>("User not found", false, HttpStatusCode.NotFound);
        }
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault()?.ToLower() ?? "unknown";
        var dto = new UserProfileDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Address = user.Address,
            Age = user.Age,
            Role = role
        };
        if (role == "seller")
        {
            var productsForSale = await _productRepository.GetByFiltered(
                p => p.UserId == userId,//bu hissede yalniz sellere aid mesullar secilir
                new Expression<Func<Product, object>>[] { },//
                isTracking: false)
                .Select(p => new ProductSimpleDto { Id = p.Id, Name = p.Name, Price = p.Price })
                .ToListAsync();
            dto.ProductsForSale = productsForSale;
        }
        else if (role == "buyer")
        {
            var purchasedProducts = await _orderRepository.GetByFiltered(
                o => o.UserId == userId,
                new Expression<Func<Order, object>>[] { o => o.OrderProducts })//cross table-e uygun olaraq eager loading ile getirir
                .SelectMany(o => o.OrderProducts)//order icerisindeki OrderProduct listini sixir
                .Select(op => new ProductSimpleDto //Hər bir OrderProduct obyektindən sadə ProductSimpleDto yaratmaq üçün map edilir.
                {
                    Id = op.ProductId,
                    Name = op.Product.Name,
                    Price = op.UnitPrice
                })
                .ToListAsync();//List seklinde alır

            dto.PurchasedProducts = purchasedProducts;//UserProfileDto-ya alinan mesullari elave edir
        }
        return new BaseResponse<UserProfileDto>(dto, true, HttpStatusCode.OK);
    }

}
