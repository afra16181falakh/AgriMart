using AgriMartAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgriMartAPI.Repositories
{
    public interface ICartRepository
    {
        Task<IEnumerable<CartItem>> GetCartItems(Guid userId);
        Task<CartItem> AddToCart(Guid userId, Guid productId, int quantity);
        Task<CartItem?> UpdateCartItemQuantity(Guid cartItemId, int newQuantity);
        Task<bool> RemoveFromCart(Guid cartItemId);
        Task ClearCart(Guid userId);
    }
}