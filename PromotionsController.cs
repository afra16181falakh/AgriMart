using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using AgriMartAPI.Models;

namespace AgriMartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PromotionsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/promotions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Promotion>>> GetPromotions()
        {
            string connectionString = _configuration.GetConnectionString("InputShopConnection")!;
            var promotions = new List<Promotion>();

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string sql = "SELECT * FROM Promotion";
                using (var command = new SqlCommand(sql, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        promotions.Add(MapReaderToPromotion(reader));
                    }
                }
            }

            return Ok(promotions);
        }

        // POST: api/promotions
        [HttpPost]
        public async Task<ActionResult<Promotion>> CreatePromotion([FromBody] Promotion promotion)
        {
            if (promotion == null)
                return BadRequest("Invalid promotion object.");

            promotion.Id = Guid.NewGuid();
            promotion.CreatedDate = DateTime.UtcNow;

            string connectionString = _configuration.GetConnectionString("InputShopConnection")!;
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string sql = @"INSERT INTO Promotion 
                               (Id, Code, Description, DiscountValue, DiscountType, StartDate, EndDate, UsageLimit, IsActive, CreatedDate, CreatedBy)
                               VALUES 
                               (@Id, @Code, @Description, @DiscountValue, @DiscountType, @StartDate, @EndDate, @UsageLimit, @IsActive, @CreatedDate, @CreatedBy)";

                using (var command = new SqlCommand(sql, connection))
                {
                    AddAllParameters(command, promotion);
                    await command.ExecuteNonQueryAsync();
                }
            }

            return Ok(promotion);
        }

        // Helper method to map data
        private Promotion MapReaderToPromotion(SqlDataReader reader)
        {
            return new Promotion
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                Code = reader.GetString(reader.GetOrdinal("Code")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                DiscountValue = reader.GetDecimal(reader.GetOrdinal("DiscountValue")),
                DiscountType = reader.GetString(reader.GetOrdinal("DiscountType")),
                StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                UsageLimit = reader.GetInt32(reader.GetOrdinal("UsageLimit")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("CreatedBy"))
            };
        }

        // Helper method to add parameters
        private void AddAllParameters(SqlCommand command, Promotion promotion)
        {
            command.Parameters.AddWithValue("@Id", promotion.Id);
            command.Parameters.AddWithValue("@Code", promotion.Code);
            command.Parameters.AddWithValue("@Description", promotion.Description);
            command.Parameters.AddWithValue("@DiscountValue", promotion.DiscountValue);
            command.Parameters.AddWithValue("@DiscountType", promotion.DiscountType);
            command.Parameters.AddWithValue("@StartDate", promotion.StartDate);
            command.Parameters.AddWithValue("@EndDate", promotion.EndDate);
            command.Parameters.AddWithValue("@UsageLimit", promotion.UsageLimit);
            command.Parameters.AddWithValue("@IsActive", promotion.IsActive);
            command.Parameters.AddWithValue("@CreatedDate", promotion.CreatedDate);

            // ✅ Proper null-safe handling
            if (string.IsNullOrEmpty(promotion.CreatedBy))
                command.Parameters.AddWithValue("@CreatedBy", DBNull.Value);
            else
                command.Parameters.AddWithValue("@CreatedBy", promotion.CreatedBy);
        }
    }
}