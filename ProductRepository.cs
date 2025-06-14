// File: src/Repositories/ProductRepository.cs

using AgriMartAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgriMartAPI.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string? _connectionString;

        public ProductRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            var products = new List<Product>();
            string sql = "SELECT Id, Name, Description, Price, StockQuantity, CategoryId, ImageUrl FROM Products";

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(new Product
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Price = reader.GetDecimal(3),
                    StockQuantity = reader.GetInt32(4),
                    CategoryId = reader.GetGuid(5),
                    ImageUrl = reader.IsDBNull(6) ? null : reader.GetString(6)
                });
            }
            return products;
        }

        public async Task<Product?> GetProductById(Guid id)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            string sql = "SELECT Id, Name, Description, Price, StockQuantity, CategoryId, ImageUrl FROM Products WHERE Id = @Id";
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);
            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Product
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Price = reader.GetDecimal(3),
                    StockQuantity = reader.GetInt32(4),
                    CategoryId = reader.GetGuid(5),
                    ImageUrl = reader.IsDBNull(6) ? null : reader.GetString(6)
                };
            }
            return null;
        }

        public async Task<Product> CreateProduct(Product product)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            string sql = "INSERT INTO Products (Id, Name, Description, Price, StockQuantity, CategoryId, ImageUrl) VALUES (@Id, @Name, @Description, @Price, @StockQuantity, @CategoryId, @ImageUrl)";
            await using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Id", product.Id);
            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Description", (object?)product.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Price", product.Price);
            command.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);
            command.Parameters.AddWithValue("@CategoryId", product.CategoryId);
            command.Parameters.AddWithValue("@ImageUrl", (object?)product.ImageUrl ?? DBNull.Value);

            await command.ExecuteNonQueryAsync();
            return product;
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            string sql = "UPDATE Products SET Name = @Name, Description = @Description, Price = @Price, StockQuantity = @StockQuantity, CategoryId = @CategoryId, ImageUrl = @ImageUrl WHERE Id = @Id";
            await using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Description", (object?)product.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Price", product.Price);
            command.Parameters.AddWithValue("@StockQuantity", product.StockQuantity);
            command.Parameters.AddWithValue("@CategoryId", product.CategoryId);
            command.Parameters.AddWithValue("@ImageUrl", (object?)product.ImageUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("@Id", product.Id);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteProduct(Guid id)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            string sql = "DELETE FROM Products WHERE Id = @Id";
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}