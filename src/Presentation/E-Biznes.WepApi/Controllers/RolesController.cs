using E_Biznes.Application.Abstract.Service;
using E_Biznes.Application.DTOs.RoleDtos;
using E_Biznes.Application.Shared;
using E_Biznes.Application.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Biznes.WepApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [Authorize(Policy = Permission.Role.GetAllPermission)]
        [ProducesResponseType(typeof(BaseResponse<List<string>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<List<string>>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseResponse<List<string>>), (int)HttpStatusCode.InternalServerError)]
        public IActionResult GetAllPermission()
        {
            var permissions = PermissionHelper.GetAllPermissions();
            return Ok(permissions);
        }
        [HttpPost]
        [Authorize(Policy = Permission.Role.Create)]
        [ProducesResponseType(typeof(BaseResponse<RoleCreateDto>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateDto dto)
        {
            var result = await _roleService.CreateRoleAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut]
        [Authorize(Policy = Permission.Role.Update)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateRole(string roleId, [FromBody] RoleCreateDto dto)
        {
            var result = await _roleService.UpdateRoleAsync(roleId, dto);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete]
        [Authorize(Policy = Permission.Role.Delete)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteRoleAsync(string roleId)
        {
            var result=await _roleService.DeleteRoleAsync(roleId);
            return StatusCode((int)result.StatusCode,result);
        }
        [HttpDelete]
        [Authorize(Policy = Permission.Role.DeletePermission)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeletePermissionInRoleAsync(string roleId,List<string> permissions)
        {
            var result = await _roleService.DeletePermissionsAsync(roleId, permissions);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
