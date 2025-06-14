using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using AgriMartAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace AgriMartAPI.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ProductReviewController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ProductReviewController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/reviews/product/{productId}
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<ProductReview>>> GetReviewsForProduct(Guid productId)
        {
            var reviews = new List<ProductReview>();
            string sql = "SELECT Id, ProductId, UserId, Rating, Comment, CreatedAt FROM ProductReviews WHERE ProductId = @ProductId";
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
                return StatusCode(500, "Connection string is missing.");

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ProductId", productId);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                reviews.Add(new ProductReview
                {
                    Id = reader.GetGuid(0),
                    ProductId = reader.GetGuid(1),
                    UserId = reader.GetGuid(2),
                    Rating = reader.GetInt32(3),
                    Comment = reader.IsDBNull(4) ? null : reader.GetString(4),
                    CreatedAt = reader.GetDateTime(5)
                });
            }

            return Ok(reviews);
        }

        // POST: api/reviews
        [HttpPost]
        public async Task<ActionResult<ProductReview>> PostProductReview([FromBody] ProductReview review)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            review.Id = Guid.NewGuid();
            review.CreatedAt = DateTime.UtcNow;

            string sql = "INSERT INTO ProductReviews (Id, ProductId, UserId, Rating, Comment, CreatedAt) " +
                         "VALUES (@Id, @ProductId, @UserId, @Rating, @Comment, @CreatedAt)";
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
                return StatusCode(500, "Connection string is missing.");

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", review.Id);
            command.Parameters.AddWithValue("@ProductId", review.ProductId);
            command.Parameters.AddWithValue("@UserId", review.UserId);
            command.Parameters.AddWithValue("@Rating", review.Rating);
            command.Parameters.AddWithValue("@Comment", (object?)review.Comment ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedAt", review.CreatedAt);

            await command.ExecuteNonQueryAsync();

            return Ok(review);
        }
    }
}