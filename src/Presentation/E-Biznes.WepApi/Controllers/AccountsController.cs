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

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpPost]
        [Authorize(Policy =Permission.Account.Create)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AdminCreateUser([FromBody] AccountRegisterDto dto)
        {
            var result = await _accountService.AdminRegisterAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }

        
    }
}
