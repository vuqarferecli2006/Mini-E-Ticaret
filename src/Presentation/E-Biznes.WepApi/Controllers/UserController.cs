using AzBinaTeam.Application.DTOs.UserDtos;
using E_Biznes.Application.Abstract.Service;
using E_Biznes.Application.DTOs.UserDtos;
using E_Biznes.Application.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Biznes.WepApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            var result = await _userService.RegisterAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var result = await _userService.LoginAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Policy = Permission.User.PasswordReset)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ResetPassword([FromBody] UserResetPasswordDto dto)
        {
            var result = await _userService.ResetPassword(dto);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(BaseResponse<TokenResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> RefreshToken([FromBody] RefrexhTokenRequestDto dto)
        {
            var result = await _userService.RefreshTokenAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet]
        [Authorize(Policy = Permission.User.SendResetEmail)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SendResetConfirmEmail([FromQuery] string email)
        {
            var result = await _userService.SendResetPasswordEmail(email);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi =true)]
        [ProducesResponseType(typeof(BaseResponse<TokenResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var result = await _userService.ConfirmEmail(userId, token);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet]
        [Authorize(Policy = Permission.User.GetMy)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetMyProfile()
        {
            var result=await _userService.GetCurrentUserProfileAsync();
            return Ok(result);
        }

    }
}
