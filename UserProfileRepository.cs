using AgriMartAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace AgriMartAPI.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly string? _connectionString;

        public UserProfileRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<UserProfile?> GetUserProfile(Guid userId)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            string sql = "SELECT Id, UserId, FirstName, LastName, Email, PhoneNumber FROM UserProfiles WHERE UserId = @UserId";
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            
            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new UserProfile {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetGuid(1),
                    FirstName = reader.GetString(2),
                    LastName = reader.GetString(3),
                    Email = reader.GetString(4),
                    PhoneNumber = reader.GetString(5)
                };
            }
            return null;
        }

        public async Task<bool> UpdateUserProfile(UserProfile userProfile)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            string sql = "UPDATE UserProfiles SET FirstName = @FirstName, LastName = @LastName, Email = @Email, PhoneNumber = @PhoneNumber WHERE Id = @Id";
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@FirstName", userProfile.FirstName);
            command.Parameters.AddWithValue("@LastName", userProfile.LastName);
            command.Parameters.AddWithValue("@Email", userProfile.Email);
            command.Parameters.AddWithValue("@PhoneNumber", userProfile.PhoneNumber);
            command.Parameters.AddWithValue("@Id", userProfile.Id);
            
            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}