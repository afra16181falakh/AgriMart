using AgriMartAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgriMartAPI.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> PlaceOrder(Guid userId, int addressId);
        Task<IEnumerable<Order>> GetUserOrders(Guid userId);
        Task<Order?> GetOrderDetails(Guid orderId);
    }
}