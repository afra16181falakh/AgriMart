using AgriMartAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgriMartAPI.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly string? _connectionString;

        public CartRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<CartItem>> GetCartItems(Guid userId)
        {
            var items = new List<CartItem>();
            // Assuming your DB table has 'Id' as the unique identifier for the cart item
            string sql = "SELECT Id, UserId, ProductId, Quantity FROM CartItems WHERE UserId = @UserId";

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                items.Add(new CartItem
                {
                    // CHANGE: Map DB's 'Id' column to model's 'CartId' property
                    CartId = reader.GetGuid(0), // Line 35 corrected
                    UserId = reader.GetGuid(1),
                    ProductId = reader.GetGuid(2),
                    Quantity = reader.GetInt32(3)
                });
            }
            return items;
        }

        public async Task<CartItem> AddToCart(Guid userId, Guid productId, int quantity)
        {
            // First, check if the item already exists in the cart
            // Assuming 'Id' here refers to the CartItemId in the DB
            string checkSql = "SELECT Id, Quantity FROM CartItems WHERE UserId = @UserId AND ProductId = @ProductId";
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            Guid? existingCartItemId = null;
            int existingQuantity = 0;

            await using (var checkCommand = new SqlCommand(checkSql, connection))
            {
                checkCommand.Parameters.AddWithValue("@UserId", userId);
                checkCommand.Parameters.AddWithValue("@ProductId", productId);
                await using (var reader = await checkCommand.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        existingCartItemId = reader.GetGuid(0);
                        existingQuantity = reader.GetInt32(1);
                    }
                }
            }

            if (existingCartItemId.HasValue)
            {
                // Item exists, update its quantity and return it
                int newQuantity = existingQuantity + quantity;
                return await UpdateCartItemQuantity(existingCartItemId.Value, newQuantity) ?? new CartItem();
            }
            else
            {
                // Item doesn't exist, insert it
                var newItem = new CartItem
                {
                    CartId = Guid.NewGuid(), // CHANGE: Use CartId here instead of Id (Line 78 corrected)
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                };
                string insertSql = "INSERT INTO CartItems (Id, UserId, ProductId, Quantity) VALUES (@Id, @UserId, @ProductId, @Quantity)";
                await using (var insertCommand = new SqlCommand(insertSql, connection))
                {
                    insertCommand.Parameters.AddWithValue("@Id", newItem.CartId); // CHANGE: Use CartId here (Line 82 corrected)
                    insertCommand.Parameters.AddWithValue("@UserId", newItem.UserId);
                    insertCommand.Parameters.AddWithValue("@ProductId", newItem.ProductId);
                    insertCommand.Parameters.AddWithValue("@Quantity", newItem.Quantity);
                    await insertCommand.ExecuteNonQueryAsync();
                }
                return newItem;
            }
        }

        public async Task<CartItem?> UpdateCartItemQuantity(Guid cartItemId, int newQuantity)
        {
            // Using a SELECT after the UPDATE to get the full, updated item back
            string sql = "UPDATE CartItems SET Quantity = @Quantity WHERE Id = @Id; SELECT Id, UserId, ProductId, Quantity FROM CartItems WHERE Id = @Id;";
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Quantity", newQuantity);
            command.Parameters.AddWithValue("@Id", cartItemId);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new CartItem
                {
                    // CHANGE: Map DB's 'Id' column to model's 'CartId' property
                    CartId = reader.GetGuid(0), // Line 108 corrected
                    UserId = reader.GetGuid(1),
                    ProductId = reader.GetGuid(2),
                    Quantity = reader.GetInt32(3)
                };
            }
            return null;
        }

        public async Task<bool> RemoveFromCart(Guid cartItemId)
        {
            string sql = "DELETE FROM CartItems WHERE Id = @Id";
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", cartItemId);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task ClearCart(Guid userId)
        {
            string sql = "DELETE FROM CartItems WHERE UserId = @UserId";
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            await command.ExecuteNonQueryAsync();
        }
    }
}