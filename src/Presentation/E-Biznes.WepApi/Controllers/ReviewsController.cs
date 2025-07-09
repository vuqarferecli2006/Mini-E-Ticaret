using E_Biznes.Application.Abstract.Service;
using E_Biznes.Application.DTOs.ReviewDtos;
using E_Biznes.Application.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace E_Biznes.WepApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        [HttpPost]
        [Authorize(Policy = Permission.Review.Create)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateReview(Guid productId, [FromBody] ReviewCreateDto dto)
        {
            var result = await _reviewService.CreateReviewAsync(productId, dto);
            return StatusCode((int)result.StatusCode, result);
        }
       
        [HttpGet]
        [Authorize(Policy = Permission.Review.GetByProduct)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetReviewsByProduct(Guid productId)
        {
            var result = await _reviewService.GetReviewByProductIdAsync(productId);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpGet]
        [Authorize(Policy = Permission.Review.GetByUser)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetReviewsByUserId(string userId)
        {
            var response = await _reviewService.GetReviewsByUserIdAsync(userId);
            return StatusCode((int)response.StatusCode, response);
        }
        [HttpDelete]
        [Authorize(Policy = Permission.Review.Delete)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteReview(Guid id)
        {
            var result = await _reviewService.DeleteAsync(id);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
