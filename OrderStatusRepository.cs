using AgriMartAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgriMartAPI.Repositories
{
    public class OrderStatusRepository : IOrderStatusRepository
    {
        private readonly string? _connectionString;

        public OrderStatusRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<OrderStatus>> GetAll()
        {
            var statuses = new List<OrderStatus>();
            string sql = "SELECT Id, StatusName FROM OrderStatuses";
            
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                statuses.Add(new OrderStatus
                {
                    Id = reader.GetGuid(0),
                    StatusName = reader.GetString(1)
                });
            }
            return statuses;
        }
    }
}