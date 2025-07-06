using E_Biznes.Application.Abstract.Service;
using E_Biznes.Application.DTOs.OrderDtos;
using E_Biznes.Application.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net;

namespace E_Biznes.WepApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var response = await _orderService.CreateOrderAsync(dto);
            return StatusCode((int)response.StatusCode, response);
        }

        // GET: /api/orders/my
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetMyOrders()
        {
            var response = await _orderService.GetMyOrdersAsync();
            return StatusCode((int)response.StatusCode, response);
        }

        // GET: /api/orders/my-sales
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetMySales()
        {
            var response = await _orderService.GetMySalesAsync();
            return StatusCode((int)response.StatusCode, response);
        }

        // GET: /api/orders/{id}
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetOrderDetail(Guid id)
        {
            var response = await _orderService.GetOrderDetailAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SendOrderEmail(
            [FromQuery] string token, 
            [FromQuery] string userId, 
            [FromQuery] Guid productId, 
            [FromQuery] int quantity,
            [FromQuery(Name = "total")] string totalStr)
        {
            if (!decimal.TryParse(totalStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var totalPrice))
                return BadRequest("TotalPrice format is incorrect");
            var result=await _orderService.SendOrderEmail(token, userId, productId, quantity, totalPrice);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
