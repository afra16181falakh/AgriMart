using AgriMartAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgriMartAPI.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string? _connectionString;
        private readonly ICartRepository _cartRepository; // Injecting another repository!

        public OrderRepository(IConfiguration configuration, ICartRepository cartRepository)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _cartRepository = cartRepository; // Store the injected cart repository
        }

        public async Task<Order> PlaceOrder(Guid userId, int addressId)
        {
            // The logic from your old controller's "place order" method moves here.
            // But instead of querying the cart directly, we use the injected repository.
            var cartItems = (List<CartItem>)await _cartRepository.GetCartItems(userId);

            if (cartItems.Count == 0)
            {
                throw new InvalidOperationException("Cannot place an order with an empty cart.");
            }

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var transaction = connection.BeginTransaction();

            try
            {
                decimal totalAmount = 0;
                // You may need a small query here to get current prices to calculate the total
                // Or you can enhance GetCartItems to return products with prices.

                // For now, let's assume totalAmount calculation happens here.

                var newOrder = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    AddressId = addressId,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = totalAmount, // You'd calculate this properly
                    Status = "Pending"
                };

                // 1. Create the Order record
                string createOrderSql = "INSERT INTO Orders (Id, UserId, AddressId, OrderDate, TotalAmount, Status) VALUES (@Id, @UserId, @AddressId, @OrderDate, @TotalAmount, @Status)";
                await using (var cmd = new SqlCommand(createOrderSql, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", newOrder.Id);
                    cmd.Parameters.AddWithValue("@UserId", newOrder.UserId);
                    cmd.Parameters.AddWithValue("@AddressId", newOrder.AddressId);
                    cmd.Parameters.AddWithValue("@OrderDate", newOrder.OrderDate);
                    cmd.Parameters.AddWithValue("@TotalAmount", newOrder.TotalAmount);
                    cmd.Parameters.AddWithValue("@Status", newOrder.Status);
                    await cmd.ExecuteNonQueryAsync();
                }

                // 2. Create OrderItem records
                string createOrderItemSql = "INSERT INTO OrderItems (Id, OrderId, ProductId, Quantity, Price) VALUES (@Id, @OrderId, @ProductId, @Quantity, @Price)";
                foreach (var item in cartItems)
                {
                    await using (var cmd = new SqlCommand(createOrderItemSql, connection, transaction))
                    {
                        // TODO: You'll need to generate a new Guid for each OrderItem.Id
                        // TODO: You'll need to add parameters for ProductId, Quantity, and Price from `item`
                        // Example:
                        cmd.Parameters.AddWithValue("@Id", Guid.NewGuid()); // Each order item needs its own ID
                        cmd.Parameters.AddWithValue("@OrderId", newOrder.Id);
                        cmd.Parameters.AddWithValue("@ProductId", item.ProductId);
                        cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                        // cmd.Parameters.AddWithValue("@Price", /* Get product price here, perhaps from a ProductRepository */);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                // 3. Clear the cart using the repository
                await _cartRepository.ClearCart(userId); // This hides the DELETE SQL from this class

                await transaction.CommitAsync();
                return newOrder;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw; // Re-throw the exception to be handled by the controller
            }
        }

        public async Task<IEnumerable<Order>> GetUserOrders(Guid userId)
        {
            // Your ADO.NET logic to get all orders for a user
            var orders = new List<Order>();
            // ... implementation ...
            await Task.CompletedTask; // Suppress CS1998 warning for now
            return orders;
        }

        public async Task<Order?> GetOrderDetails(Guid orderId)
        {
            // Your ADO.NET logic to get a single order and its associated order items
            Order? order = null;
            // ... implementation ...
            await Task.CompletedTask; // Suppress CS1998 warning for now
            return order;
        }
    }
}