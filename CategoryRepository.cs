using AgriMartAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgriMartAPI.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly string? _connectionString;

        public CategoryRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            var categories = new List<Category>();
            string sql = "SELECT Id, Name, Description FROM Categories";

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                categories.Add(new Category
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                });
            }
            return categories;
        }

        public async Task<Category?> GetCategoryById(Guid id)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            string sql = "SELECT Id, Name, Description FROM Categories WHERE Id = @Id";
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.Add(new SqlParameter("@Id", System.Data.SqlDbType.UniqueIdentifier) { Value = id });

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Category
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                };
            }
            return null;
        }

        public async Task<Category> CreateCategory(Category category)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            string sql = "INSERT INTO Categories (Id, Name, Description) VALUES (@Id, @Name, @Description)";
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", category.Id);
            command.Parameters.AddWithValue("@Name", category.Name);
            command.Parameters.AddWithValue("@Description", (object?)category.Description ?? DBNull.Value);

            await command.ExecuteNonQueryAsync();

            return category;
        }

        public async Task<bool> UpdateCategory(Category category)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            string sql = "UPDATE Categories SET Name = @Name, Description = @Description WHERE Id = @Id";
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Name", category.Name);
            command.Parameters.AddWithValue("@Description", (object?)category.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@Id", category.Id);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteCategory(Guid id)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            string sql = "DELETE FROM Categories WHERE Id = @Id";
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.Add(new SqlParameter("@Id", System.Data.SqlDbType.UniqueIdentifier) { Value = id });

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}