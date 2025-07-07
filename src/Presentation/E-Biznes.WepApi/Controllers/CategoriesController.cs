using E_Biznes.Application.Abstract.Service;
using E_Biznes.Application.DTOs.CategoryDtos;
using E_Biznes.Application.Shared;
using E_Biznes.Application.Validations.CategoryValidations;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Biznes.WepApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;

        }

        [HttpPost]
        [Authorize(Policy = Permission.Category.MainCategoryCreate)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]

        public async Task<IActionResult> AddMainCategoryAsync([FromBody] CategoryMainCreateDto dto)
        {
            var result = await _categoryService.AddMainCategoryAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Policy = Permission.Category.SubCategoryCreate)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddSubCategoryAsync([FromBody] CategorySubCreateDto dto)
        {
            var result = await _categoryService.AddSubCategoryAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }


        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<List<CategoryMainGetDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<List<CategoryMainGetDto>>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseResponse<List<CategoryMainGetDto>>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _categoryService.GetAllAsync();
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<List<CategoryMainGetDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<List<CategoryMainGetDto>>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseResponse<List<CategoryMainGetDto>>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetByIdAsync([FromQuery] Guid id)
        {
            var result=await _categoryService.GetByIdAsync(id);
            return StatusCode((int)result.StatusCode,result);
        }
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<List<CategoryMainGetDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<List<CategoryMainGetDto>>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseResponse<List<CategoryMainGetDto>>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetByNameAsync([FromQuery] string search)
        {
            var result = await _categoryService.GetByNameAsync(search);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpPut]
        [Authorize(Policy = Permission.Category.MainCategoryUpdate)]
        [ProducesResponseType(typeof(BaseResponse<List<CategoryMainGetDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<List<CategoryMainGetDto>>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseResponse<List<CategoryMainGetDto>>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateMainCategory([FromBody] MainCategoryUpdateDto dto)
        {

            var result = await _categoryService.UpdateMainCategoryAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut]
        [Authorize(Policy = Permission.Category.SubCategoryUpdate)]
        [ProducesResponseType(typeof(BaseResponse<List<CategoryMainGetDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<List<CategoryMainGetDto>>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseResponse<List<CategoryMainGetDto>>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateSubCategory([FromBody] SubCategoryUpdateDto dto)
        {
            var result = await _categoryService.UpdateSubCategoryAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }



        [HttpDelete]
        [Authorize(Policy = Permission.Category.Delete)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]

        public async Task<IActionResult> DeleteAsync([FromBody] Guid id)
        {
            var result = await _categoryService.DeleteAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
