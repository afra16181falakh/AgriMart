using AgriMartAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AgriMartAPI.Controllers
{
    public class AddToWishlistRequest
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
    }
    
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistRepository _wishlistRepository;

        public WishlistController(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetWishlistItems(Guid userId)
        {
            var items = await _wishlistRepository.GetWishlistItems(userId);
            return Ok(items);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistRequest request)
        {
            // Prevent adding duplicates
            if (await _wishlistRepository.IsItemInWishlist(request.UserId, request.ProductId))
            {
                return Conflict("This item is already in the wishlist.");
            }
            
            var newItem = await _wishlistRepository.AddToWishlist(request.UserId, request.ProductId);
            
            if(newItem == null)
            {
                return StatusCode(500, "An error occurred while adding the item to the wishlist.");
            }

            return Ok(new { message = "Item added to wishlist successfully.", item = newItem });
        }

        [HttpDelete("remove/{wishlistItemId}")]
        public async Task<IActionResult> RemoveFromWishlist(Guid wishlistItemId)
        {
            var success = await _wishlistRepository.RemoveFromWishlist(wishlistItemId);
            if (!success)
            {
                return NotFound("Wishlist item not found.");
            }
            return NoContent();
        }
    }
}