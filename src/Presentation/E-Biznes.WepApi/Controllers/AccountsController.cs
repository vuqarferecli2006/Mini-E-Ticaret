using AzBinaTeam.Application.DTOs.UserDtos;
using E_Biznes.Application.Abstract.Service;
using E_Biznes.Application.DTOs.AccountsDto;
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
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;

        public AccountsController(IAccountService accountService, IUserService userService)
        {
            _accountService = accountService;
            _userService = userService;
        }
        [HttpPost]
        [Authorize(Policy =Permission.Account.Create)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AdminCreateUser([FromBody] AccountRegisterDto dto)
        {
            var result = await _accountService.RegisterAdminAccountAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpPost("assign-roles")]
        [Authorize(Policy =Permission.Account.AddRole)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddRole([FromBody] UserAddRoleDto dto)
        {
            var result = await _userService.AddRole(dto);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet]
        [Authorize(Policy = Permission.User.GetAll)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var result = await _userService.GetAllAsync();
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpGet]
        [Authorize(Policy = Permission.User.GetById)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetById([FromQuery] AccountGetDto dto)
        {
            var result = await _accountService.GetByIdAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
