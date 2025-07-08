using E_Biznes.Application.Abstract.Service;
using E_Biznes.Application.DTOs.ProducDtos;
using E_Biznes.Application.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace E_Biznes.WepApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [Authorize(Policy = Permission.Product.Create)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateProductWithImages([FromForm] ProductCreateWithImagesDto dto)
        {
            var result = await _productService.CreateWithImagesAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpPost]
        [Authorize(Policy = Permission.Product.AddProductImage)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddProductImage([FromQuery] Guid productId, IFormFile file)
        {
            var result = await _productService.AddProductImageAsync(productId, file);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpPost]
        [Authorize(Policy = Permission.Product.AddProductFavourite)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddProductFavourite([FromQuery] Guid productId)
        {
            var result = await _productService.AddProductFavouriteAsync(productId);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpPost]
        [Authorize(Policy = Permission.Product.AddProductDisCount)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddProductDisCount(Guid productId, decimal disCount)
        {
            var result = await _productService.AddProductDisCount(productId,disCount);
            return StatusCode((int)result.StatusCode, result);
        }
        
        [HttpGet]
        [Authorize(Policy = Permission.Product.GetAll)]
        [ProducesResponseType(typeof(BaseResponse<List<ProductGetDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _productService.GetAllAsync();
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpGet]
        [Authorize(Policy = Permission.Product.GetMy)]
        [ProducesResponseType(typeof(BaseResponse<List<ProductGetDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetMyProducts()
        {
            var result = await _productService.GetMyProductsAsync();
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetFiltered([FromQuery] ProductFilterParams filter)
        {
            var result = await _productService.GetFilteredProductsAsync(filter);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpGet]
        [Authorize(Policy = Permission.Product.GetAllFavourite)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllFavouriteAsync()
        {
            var result = await _productService.GetAllFavouritesAsync();
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpPut]
        [Authorize(Policy = Permission.Product.Update)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateProductWithImages([FromForm] ProductUpdateWithImagesDto dto)
        {
            var result = await _productService.UpdateWithImagesAsync(dto);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpDelete]
        [Authorize(Policy = Permission.Product.DeleteProductImage)]
        [ProducesResponseType(typeof(BaseResponse<List<ProductGetDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteProductImage([FromQuery] Guid imageId)
        {
            var result = await _productService.DeleteImageAsync(imageId);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpDelete]
        [Authorize(Policy = Permission.Product.CancelProductDisCount)]
        [ProducesResponseType(typeof(BaseResponse<List<ProductGetDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CancelProductDisCount(Guid porductId)
        {
            var result = await _productService.CancelProductDisCount(porductId);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpDelete]
        [Authorize(Policy = Permission.Product.Deletefavourite)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteFavouriteAscync(Guid id)
        {
            var result = await _productService.DeleteProductFavouriteAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpDelete]
        [Authorize(Policy = Permission.Product.Delete)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteProduct([FromQuery] Guid id)
        {
            var result = await _productService.DeleteAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }
        
    }
}
