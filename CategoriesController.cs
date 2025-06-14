using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using AgriMartAPI.Models;

namespace AgriMartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CategoriesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            string connectionString = _configuration.GetConnectionString("InputShopConnection")!;
            var categories = new List<Category>();

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            string sql = "SELECT * FROM Category";

            using var command = new SqlCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                categories.Add(MapReaderToCategory(reader));
            }

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategoryById(Guid id)
        {
            Category? category = null;
            string connectionString = _configuration.GetConnectionString("InputShopConnection")!;
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            string sql = "SELECT * FROM Category WHERE Id = @Id";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = id });

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                category = MapReaderToCategory(reader);
            }

            return category == null ? NotFound() : Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] Category category)
        {
            category.Id = Guid.NewGuid();
            category.CreatedDate = DateTime.UtcNow;

            string connectionString = _configuration.GetConnectionString("InputShopConnection")!;
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            string sql = @"
                INSERT INTO Category (
                    Id, Name, Description, ImageUrl, ParentCategoryId, SortOrder, 
                    StatusId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate
                ) VALUES (
                    @Id, @Name, @Description, @ImageUrl, @ParentCategoryId, @SortOrder, 
                    @StatusId, @CreatedBy, @CreatedDate, @ModifiedBy, @ModifiedDate
                )";

            using var command = new SqlCommand(sql, connection);
            AddAllParameters(command, category);
            await command.ExecuteNonQueryAsync();

            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] Category category)
        {
            if (id != category.Id)
                return BadRequest("ID mismatch");

            category.ModifiedDate = DateTime.UtcNow;

            string connectionString = _configuration.GetConnectionString("InputShopConnection")!;
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            string sql = @"
                UPDATE Category SET 
                    Name = @Name, Description = @Description, ImageUrl = @ImageUrl, 
                    ParentCategoryId = @ParentCategoryId, SortOrder = @SortOrder, 
                    StatusId = @StatusId, ModifiedBy = @ModifiedBy, ModifiedDate = @ModifiedDate 
                WHERE Id = @Id";

            using var command = new SqlCommand(sql, connection);
            AddAllParameters(command, category);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected == 0 ? NotFound() : NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            string connectionString = _configuration.GetConnectionString("InputShopConnection")!;
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            string sql = "DELETE FROM Category WHERE Id = @Id";
            using var command = new SqlCommand(sql, connection);
            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = id });

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected == 0 ? NotFound() : NoContent();
        }

        private Category MapReaderToCategory(SqlDataReader reader)
        {
            return new Category
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl")),
                ParentCategoryId = reader.IsDBNull(reader.GetOrdinal("ParentCategoryId")) ? null : reader.GetGuid(reader.GetOrdinal("ParentCategoryId")),
                SortOrder = reader.IsDBNull(reader.GetOrdinal("SortOrder")) ? null : reader.GetInt32(reader.GetOrdinal("SortOrder")),
                StatusId = reader.IsDBNull(reader.GetOrdinal("StatusId")) ? null : reader.GetInt32(reader.GetOrdinal("StatusId")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                ModifiedBy = reader.IsDBNull(reader.GetOrdinal("ModifiedBy")) ? null : reader.GetString(reader.GetOrdinal("ModifiedBy")),
                ModifiedDate = reader.IsDBNull(reader.GetOrdinal("ModifiedDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ModifiedDate"))
            };
        }

        private void AddAllParameters(SqlCommand command, Category category)
        {
            command.Parameters.AddWithValue("@Id", category.Id);
            command.Parameters.AddWithValue("@Name", category.Name);
            command.Parameters.AddWithValue("@CreatedDate", category.CreatedDate);

            command.Parameters.AddWithValue("@Description", (object?)category.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@ImageUrl", (object?)category.ImageUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("@ParentCategoryId", (object?)category.ParentCategoryId ?? DBNull.Value);
            command.Parameters.AddWithValue("@SortOrder", (object?)category.SortOrder ?? DBNull.Value);
            command.Parameters.AddWithValue("@StatusId", (object?)category.StatusId ?? DBNull.Value);
            command.Parameters.AddWithValue("@CreatedBy", (object?)category.CreatedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@ModifiedBy", (object?)category.ModifiedBy ?? DBNull.Value);
            command.Parameters.AddWithValue("@ModifiedDate", (object?)category.ModifiedDate ?? DBNull.Value);
        }
    }
}