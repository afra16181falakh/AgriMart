using AgriMartAPI.Models;
using AgriMartAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AgriMartAPI.Controllers
{
    public class AddToCartRequest
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartRequest
    {
        public int Quantity { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(Guid userId)
        {
            var items = await _cartRepository.GetCartItems(userId);
            return Ok(items);
        }

        [HttpPost("item")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            if (request.Quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }
            var cartItem = await _cartRepository.AddToCart(request.UserId, request.ProductId, request.Quantity);
            return Ok(cartItem);
        }

        [HttpPut("item/{cartItemId}")]
        public async Task<IActionResult> UpdateCartItem(Guid cartItemId, [FromBody] UpdateCartRequest request)
        {
            if (request.Quantity <= 0)
            {
                // If quantity is 0 or less, remove the item instead
                await _cartRepository.RemoveFromCart(cartItemId);
                return NoContent();
            }
            var updatedItem = await _cartRepository.UpdateCartItemQuantity(cartItemId, request.Quantity);
            if (updatedItem == null)
            {
                return NotFound("Cart item not found.");
            }
            return Ok(updatedItem);
        }

        [HttpDelete("item/{cartItemId}")]
        public async Task<IActionResult> RemoveFromCart(Guid cartItemId)
        {
            var success = await _cartRepository.RemoveFromCart(cartItemId);
            if (!success)
            {
                return NotFound("Cart item not found.");
            }
            return NoContent();
        }
    }
}