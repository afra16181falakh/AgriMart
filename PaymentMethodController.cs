using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using AgriMartAPI.Models;
using System;

namespace MyInputShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PaymentMethodController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentMethod>>> GetPaymentMethods()
        {
            var methods = new List<PaymentMethod>();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                return StatusCode(500, "Connection string is missing.");

            string sql = "SELECT Id, Name FROM PaymentMethod";

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                methods.Add(new PaymentMethod
                {
                    Id = reader.GetGuid(0), // ✅ Correct for UNIQUEIDENTIFIER
                    Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1)
                });
            }

            return Ok(methods);
        }
    }
}