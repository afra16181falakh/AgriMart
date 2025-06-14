// File: Controllers/PaymentTransactionController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using AgriMartAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace AgriMartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentTransactionController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PaymentTransactionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/PaymentTransaction/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<PaymentTransaction>>> GetTransactionsForUser(Guid userId)
        {
            var transactions = new List<PaymentTransaction>();

            string sql = @"
                SELECT p.Id, p.OrderId, p.Amount, p.StatusId, p.TransactionDate, p.PaymentMethodId
                FROM PaymentTransaction p
                INNER JOIN [Order] o ON p.OrderId = o.Id
                WHERE o.UserId = @UserId";

            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
                return StatusCode(500, "Database connection string is missing.");

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                transactions.Add(new PaymentTransaction
                {
                    Id = reader.GetGuid(0),
                    OrderId = reader.IsDBNull(1) ? null : reader.GetGuid(1),
                    Amount = reader.GetDecimal(2),
                    StatusId = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    TransactionDate = reader.GetDateTime(4),
                    PaymentMethodId = reader.IsDBNull(5) ? null : reader.GetGuid(5)
                });
            }

            return Ok(transactions);
        }
    }
}