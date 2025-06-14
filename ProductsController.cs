using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using AgriMartAPI.Models;

namespace AgriMartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ProductsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            string connectionString = _configuration.GetConnectionString("InputShopConnection")!;
            var products = new List<Product>();
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string sql = "SELECT * FROM Product";
                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            products.Add(MapReaderToProduct(reader));
                        }
                    }
                }
            }
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(Guid id)
        {
            Product? product = null;
            string connectionString = _configuration.GetConnectionString("InputShopConnection")!;
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string sql = "SELECT * FROM Product WHERE Id = @Id";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = id });
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            product = MapReaderToProduct(reader);
                        }
                    }
                }
            }
            return product == null ? NotFound() : Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            if (product == null) return BadRequest();

            product.Id = Guid.NewGuid();
            product.CreatedDate = DateTime.UtcNow;

            string connectionString = _configuration.GetConnectionString("InputShopConnection")!;
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string sql = @"INSERT INTO Product 
                (Id, Name, Description, Price, SKU, StockQuantity, ImageUrl1, ImageUrl2, ImageUrl3, CategoryId, IsFeatured, StatusId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
                VALUES 
                (@Id, @Name, @Description, @Price, @SKU, @StockQuantity, @ImageUrl1, @ImageUrl2, @ImageUrl3, @CategoryId, @IsFeatured, @StatusId, @CreatedBy, @CreatedDate, @ModifiedBy, @ModifiedDate)";

                using (var command = new SqlCommand(sql, connection))
                {
                    AddAllParameters(command, product);
                    await command.ExecuteNonQueryAsync();
                }
            }
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] Product product)
        {
            if (product == null || id != product.Id)
                return BadRequest("ID mismatch or null product");

            product.ModifiedDate = DateTime.UtcNow;
            string connectionString = _configuration.GetConnectionString("InputShopConnection")!;
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string sql = @"UPDATE Product SET 
                    Name = @Name, Description = @Description, Price = @Price, SKU = @SKU,
                    StockQuantity = @StockQuantity, ImageUrl1 = @ImageUrl1, ImageUrl2 = @ImageUrl2, ImageUrl3 = @ImageUrl3,
                    CategoryId = @CategoryId, IsFeatured = @IsFeatured, StatusId = @StatusId, ModifiedBy = @ModifiedBy, ModifiedDate = @ModifiedDate 
                    WHERE Id = @Id";

                using (var command = new SqlCommand(sql, connection))
                {
                    AddAllParameters(command, product);
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0) return NotFound();
                }
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            string connectionString = _configuration.GetConnectionString("InputShopConnection")!;
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string sql = "DELETE FROM Product WHERE Id = @Id";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = id });
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0) return NotFound();
                }
            }
            return NoContent();
        }

        private Product MapReaderToProduct(SqlDataReader reader)
        {
            return new Product
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                SKU = reader.IsDBNull(reader.GetOrdinal("SKU")) ? null : reader.GetString(reader.GetOrdinal("SKU")),
                StockQuantity = reader.GetInt32(reader.GetOrdinal("StockQuantity")),
                ImageUrl1 = reader.IsDBNull(reader.GetOrdinal("ImageUrl1")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl1")),
                ImageUrl2 = reader.IsDBNull(reader.GetOrdinal("ImageUrl2")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl2")),
                ImageUrl3 = reader.IsDBNull(reader.GetOrdinal("ImageUrl3")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl3")),
                CategoryId = reader.IsDBNull(reader.GetOrdinal("CategoryId")) ? null : reader.GetGuid(reader.GetOrdinal("CategoryId")),
                IsFeatured = reader.GetBoolean(reader.GetOrdinal("IsFeatured")),
                StatusId = reader.IsDBNull(reader.GetOrdinal("StatusId")) ? null : reader.GetInt32(reader.GetOrdinal("StatusId")),
                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                ModifiedBy = reader.IsDBNull(reader.GetOrdinal("ModifiedBy")) ? null : reader.GetString(reader.GetOrdinal("ModifiedBy")),
                ModifiedDate = reader.IsDBNull(reader.GetOrdinal("ModifiedDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ModifiedDate"))
            };
        }

        private void AddAllParameters(SqlCommand command, Product product)
        {
            command.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = product.Id });
            command.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar) { Value = product.Name ?? string.Empty });
            command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar) { Value = (object?)product.Description ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@Price", SqlDbType.Decimal) { Value = product.Price });
            command.Parameters.Add(new SqlParameter("@SKU", SqlDbType.NVarChar) { Value = (object?)product.SKU ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@StockQuantity", SqlDbType.Int) { Value = product.StockQuantity });
            command.Parameters.Add(new SqlParameter("@ImageUrl1", SqlDbType.NVarChar) { Value = (object?)product.ImageUrl1 ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@ImageUrl2", SqlDbType.NVarChar) { Value = (object?)product.ImageUrl2 ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@ImageUrl3", SqlDbType.NVarChar) { Value = (object?)product.ImageUrl3 ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@CategoryId", SqlDbType.UniqueIdentifier) { Value = (object?)product.CategoryId ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@IsFeatured", SqlDbType.Bit) { Value = product.IsFeatured });
            command.Parameters.Add(new SqlParameter("@StatusId", SqlDbType.Int) { Value = (object?)product.StatusId ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.NVarChar) { Value = (object?)product.CreatedBy ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@CreatedDate", SqlDbType.DateTime) { Value = product.CreatedDate });
            command.Parameters.Add(new SqlParameter("@ModifiedBy", SqlDbType.NVarChar) { Value = (object?)product.ModifiedBy ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@ModifiedDate", SqlDbType.DateTime) { Value = (object?)product.ModifiedDate ?? DBNull.Value });
        }
    }
}