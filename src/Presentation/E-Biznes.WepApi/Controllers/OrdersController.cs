﻿using E_Biznes.Application.Abstract.Service;
using E_Biznes.Application.DTOs.OrderDtos;
using E_Biznes.Application.Shared;
using E_Biznes.Domain.Enum;
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
        [Authorize(Policy = Permission.Order.Create)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto dto)
        {
            var response = await _orderService.CreateOrderAsync(dto);
            return StatusCode((int)response.StatusCode, response);
        }

        // GET: /api/orders/my
        [HttpGet]
        [Authorize(Policy = Permission.Order.GetMy)]
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
        [Authorize(Policy = Permission.Order.GetMySales)]
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
        [Authorize(Policy = Permission.Order.GetDetail)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetOrderDetail(Guid id)
        {
            var response = await _orderService.GetOrderDetailAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
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
            var result = await _orderService.SendOrderEmail(token, userId, productId, quantity, totalPrice);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SellerOrderDetails(
        string token,
        string buyerId,
        Guid productId,
        int quantity,
        decimal total)
        {
            var response = await _orderService.GetSellerOrderDetailsAsync(token, buyerId, productId, quantity, total);
            return StatusCode((int)response.StatusCode, response);
        }
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult ViewStatusChange(Guid orderId, string oldStatus, string newStatus)
        {
            var html = _orderService.GenerateStatusChangeHtml(orderId, oldStatus, newStatus);
            return Content(html, "text/html");
        }
        [HttpPut]
        [Authorize(Policy = Permission.Order.Update)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateStatus([FromQuery] Guid orderId, [FromQuery] OrderStatus newStatus)
        {
            var response = await _orderService.ChangeOrderStatusAsync(orderId, newStatus);
            return StatusCode((int)response.StatusCode, response);
        }
        [HttpDelete]
        [Authorize(Policy = Permission.Order.Delete)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteOrderAsync(Guid orderId)
        {
            var result = await _orderService.CancelAndNotifyOrderAsync(orderId);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
