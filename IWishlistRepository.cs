using AgriMartAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgriMartAPI.Repositories
{
    public interface IWishlistRepository
    {
        Task<IEnumerable<WishlistItem>> GetWishlistItems(Guid userId);
        Task<WishlistItem?> AddToWishlist(Guid userId, Guid productId);
        Task<bool> RemoveFromWishlist(Guid wishlistItemId);
        Task<bool> IsItemInWishlist(Guid userId, Guid productId);
    }
}