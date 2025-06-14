using AgriMartAPI.Models;
using AgriMartAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AgriMartAPI.Controllers
{
    public class PlaceOrderRequest
    {
        public Guid UserId { get; set; }
        public int AddressId { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpPost("place")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            try
            {
                var newOrder = await _orderRepository.PlaceOrder(request.UserId, request.AddressId);
                return Ok(new { message = "Order placed successfully!", orderId = newOrder.Id });
            }
            catch (InvalidOperationException ex)
            {
                // Catches specific, expected errors like an empty cart
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                // Catches all other unexpected database or system errors
                return StatusCode(500, "An internal error occurred while placing the order.");
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserOrders(Guid userId)
        {
            var orders = await _orderRepository.GetUserOrders(userId);
            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderDetails(Guid orderId)
        {
            var order = await _orderRepository.GetOrderDetails(orderId);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }
    }
}