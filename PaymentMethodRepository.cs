using AgriMartAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgriMartAPI.Repositories
{
    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly string? _connectionString;

        public PaymentMethodRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<PaymentMethod>> GetActiveMethods()
        {
            var methods = new List<PaymentMethod>();
            // Assuming you have an IsActive column
            string sql = "SELECT Id, Name, Description FROM PaymentMethods WHERE IsActive = 1";
            
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                methods.Add(new PaymentMethod
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    Description = reader.GetString(2)
                });
            }
            return methods;
        }
    }
}