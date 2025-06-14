using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using AgriMartAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgriMartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryChargeRulesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DeliveryChargeRulesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeliveryChargeRule>>> GetDeliveryChargeRules()
        {
            // THE FIX IS HERE: We look for "InputShopConnection" to match your appsettings.json
            string? connectionString = _configuration.GetConnectionString("InputShopConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                return StatusCode(500, "Internal server error: Database connection string is not configured.");
            }

            var deliveryRules = new List<DeliveryChargeRule>();

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                
                string sql = "SELECT Id, RuleName, Description, ChargeAmount, SortOrder, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate FROM DeliveryChargeRule";
                
                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var rule = new DeliveryChargeRule
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                RuleName = reader.GetString(reader.GetOrdinal("RuleName")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                ChargeAmount = reader.GetDecimal(reader.GetOrdinal("ChargeAmount")),
                                SortOrder = reader.IsDBNull(reader.GetOrdinal("SortOrder")) ? null : reader.GetInt32(reader.GetOrdinal("SortOrder")),
                                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                                ModifiedBy = reader.IsDBNull(reader.GetOrdinal("ModifiedBy")) ? null : reader.GetString(reader.GetOrdinal("ModifiedBy")),
                                ModifiedDate = reader.IsDBNull(reader.GetOrdinal("ModifiedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ModifiedDate"))
                            };
                            deliveryRules.Add(rule);
                        }
                    }
                }
            }
            return Ok(deliveryRules);
        }
    }
}