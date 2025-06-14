using AgriMartAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgriMartAPI.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly string? _connectionString;

        public WishlistRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<WishlistItem>> GetWishlistItems(Guid userId)
        {
            var items = new List<WishlistItem>();
            string sql = "SELECT Id, UserId, ProductId FROM WishlistItems WHERE UserId = @UserId";

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                items.Add(new WishlistItem
                {
                    Id = reader.GetGuid(0),
                    UserId = reader.GetGuid(1),
                    ProductId = reader.GetGuid(2)
                });
            }
            return items;
        }

        public async Task<WishlistItem?> AddToWishlist(Guid userId, Guid productId)
        {
            var newItem = new WishlistItem
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ProductId = productId
            };

            string sql = "INSERT INTO WishlistItems (Id, UserId, ProductId) VALUES (@Id, @UserId, @ProductId)";

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", newItem.Id);
            command.Parameters.AddWithValue("@UserId", newItem.UserId);
            command.Parameters.AddWithValue("@ProductId", newItem.ProductId);

            await command.ExecuteNonQueryAsync();
            return newItem;
        }

        public async Task<bool> RemoveFromWishlist(Guid wishlistItemId)
        {
            string sql = "DELETE FROM WishlistItems WHERE Id = @Id";

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", wishlistItemId);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> IsItemInWishlist(Guid userId, Guid productId)
        {
            string sql = "SELECT COUNT(1) FROM WishlistItems WHERE UserId = @UserId AND ProductId = @ProductId";

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@ProductId", productId);

            // CORRECTED LINE FOR CS8605 WARNING:
            object? result = await command.ExecuteScalarAsync();
            // Use Convert.ToInt32 to safely handle DBNull.Value and null, converting them to 0
            int count = Convert.ToInt32(result);

            return count > 0;
        }
    }
}