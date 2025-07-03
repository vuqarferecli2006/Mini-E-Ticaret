using E_Biznes.Application.Abstract.Service;
using E_Biznes.Application.DTOs.RoleDtos;
using E_Biznes.Application.Shared.Helpers;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("permission")]
        public IActionResult GetAllPermission()
        {
            var permissions = PermissionHelper.GetAllPermissions();
            return Ok(permissions);
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateDto dto)
        {
            var result = await _roleService.CreateRoleAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRole(string roleId, [FromBody] RoleCreateDto dto)
        {
            var result = await _roleService.UpdateRoleAsync(roleId, dto);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
