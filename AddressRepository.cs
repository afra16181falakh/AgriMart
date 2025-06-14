using AgriMartAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgriMartAPI.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly string? _connectionString;

        public AddressRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Renamed to match the interface and controller (singular "Address")
        public async Task<IEnumerable<Address>> GetAddressByUserId(Guid userId)
        {
            var addresses = new List<Address>();
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            string sql = "SELECT * FROM Address WHERE UserId = @UserId";
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            await using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    addresses.Add(MapReaderToAddress(reader));
                }
            }
            return addresses;
        }

        // Changed id type from int to Guid to match Address model and interface
        public async Task<Address?> GetAddressById(Guid id)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            string sql = "SELECT * FROM Address WHERE Id = @Id";
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id); // ID is now Guid

            await using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return MapReaderToAddress(reader);
                }
            }
            return null;
        }

        public async Task<Address> CreateAddress(Address address)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            // Removed "OUTPUT INSERTED.Id" since ID is generated client-side with Guid.NewGuid()
            string sql = @"INSERT INTO Address (Id, UserId, FullName, PhoneNumber, AddressLine1, AddressLine2, City, State, PostalCode, Country, AddressType, IsDefaultShipping, IsDefaultBilling, CreatedDate)
                           VALUES (@Id, @UserId, @FullName, @PhoneNumber, @AddressLine1, @AddressLine2, @City, @State, @PostalCode, @Country, @AddressType, @IsDefaultShipping, @IsDefaultBilling, @CreatedDate)";

            await using (var command = new SqlCommand(sql, connection))
            {
                address.Id = Guid.NewGuid(); // Ensure new ID is generated
                address.CreatedDate = DateTime.UtcNow;

                command.Parameters.AddWithValue("@Id", address.Id);
                command.Parameters.AddWithValue("@UserId", address.UserId);
                command.Parameters.AddWithValue("@FullName", address.FullName);
                command.Parameters.AddWithValue("@PhoneNumber", address.PhoneNumber);
                command.Parameters.AddWithValue("@AddressLine1", address.AddressLine1);
                command.Parameters.AddWithValue("@AddressLine2", (object?)address.AddressLine2 ?? DBNull.Value);
                command.Parameters.AddWithValue("@City", address.City);
                command.Parameters.AddWithValue("@State", address.State);
                command.Parameters.AddWithValue("@PostalCode", address.PostalCode);
                command.Parameters.AddWithValue("@Country", address.Country);
                command.Parameters.AddWithValue("@AddressType", (object?)address.AddressType ?? DBNull.Value);
                command.Parameters.AddWithValue("@IsDefaultShipping", address.IsDefaultShipping);
                command.Parameters.AddWithValue("@IsDefaultBilling", address.IsDefaultBilling);
                command.Parameters.AddWithValue("@CreatedDate", address.CreatedDate);

                await command.ExecuteNonQueryAsync();
            }
            return address;
        }

        public async Task<bool> UpdateAddress(Address address)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            string sql = @"UPDATE Address SET
                           UserId = @UserId, FullName = @FullName, PhoneNumber = @PhoneNumber, AddressLine1 = @AddressLine1, AddressLine2 = @AddressLine2,
                           City = @City, State = @State, PostalCode = @PostalCode, Country = @Country, AddressType = @AddressType,
                           IsDefaultShipping = @IsDefaultShipping, IsDefaultBilling = @IsDefaultBilling, ModifiedDate = @ModifiedDate
                           WHERE Id = @Id";

            await using (var command = new SqlCommand(sql, connection))
            {
                address.ModifiedDate = DateTime.UtcNow;

                command.Parameters.AddWithValue("@UserId", address.UserId);
                command.Parameters.AddWithValue("@FullName", address.FullName);
                command.Parameters.AddWithValue("@PhoneNumber", address.PhoneNumber);
                command.Parameters.AddWithValue("@AddressLine1", address.AddressLine1);
                command.Parameters.AddWithValue("@AddressLine2", (object?)address.AddressLine2 ?? DBNull.Value);
                command.Parameters.AddWithValue("@City", address.City);
                command.Parameters.AddWithValue("@State", address.State);
                command.Parameters.AddWithValue("@PostalCode", address.PostalCode);
                command.Parameters.AddWithValue("@Country", address.Country);
                command.Parameters.AddWithValue("@AddressType", (object?)address.AddressType ?? DBNull.Value);
                command.Parameters.AddWithValue("@IsDefaultShipping", address.IsDefaultShipping);
                command.Parameters.AddWithValue("@IsDefaultBilling", address.IsDefaultBilling);
                command.Parameters.AddWithValue("@ModifiedDate", address.ModifiedDate);
                command.Parameters.AddWithValue("@Id", address.Id);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

        // Changed id type from int to Guid to match Address model and interface
        public async Task<bool> DeleteAddress(Guid id)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            string sql = "DELETE FROM Address WHERE Id = @Id";
            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id); // ID is now Guid

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        // Helper method to avoid repeating code - COMPLETED MAPPING ALL PROPERTIES
        private Address MapReaderToAddress(SqlDataReader reader)
        {
            return new Address
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                FullName = reader.GetString(reader.GetOrdinal("FullName")),
                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                AddressLine1 = reader.GetString(reader.GetOrdinal("AddressLine1")),
                AddressLine2 = reader.IsDBNull(reader.GetOrdinal("AddressLine2")) ? null : reader.GetString(reader.GetOrdinal("AddressLine2")),
                City = reader.GetString(reader.GetOrdinal("City")),
                State = reader.GetString(reader.GetOrdinal("State")),
                PostalCode = reader.GetString(reader.GetOrdinal("PostalCode")),
                Country = reader.GetString(reader.GetOrdinal("Country")),
                AddressType = reader.IsDBNull(reader.GetOrdinal("AddressType")) ? null : reader.GetString(reader.GetOrdinal("AddressType")),
                IsDefaultShipping = reader.GetBoolean(reader.GetOrdinal("IsDefaultShipping")),
                IsDefaultBilling = reader.GetBoolean(reader.GetOrdinal("IsDefaultBilling")),
                StatusId = reader.IsDBNull(reader.GetOrdinal("StatusId")) ? null : reader.GetInt32(reader.GetOrdinal("StatusId")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                ModifiedBy = reader.IsDBNull(reader.GetOrdinal("ModifiedBy")) ? null : reader.GetString(reader.GetOrdinal("ModifiedBy")),
                ModifiedDate = reader.IsDBNull(reader.GetOrdinal("ModifiedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ModifiedDate"))
            };
        }
    }
}