// File: Controllers/CmsPageController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using AgriMartAPI.Models; 
using System.Threading.Tasks;

namespace MyInputShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CmsPageController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CmsPageController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/CmsPage/by-slug/{slug}
        [HttpGet("by-slug/{slug}")]
        public async Task<ActionResult<CmsPage>> GetCmsPage(string slug)
        {
            CmsPage? cmsPage = null;
            string sql = "SELECT Id, Title, Slug, Content, CreatedAt, UpdatedAt FROM CmsPages WHERE Slug = @Slug";
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
                return StatusCode(500, "Connection string 'DefaultConnection' not found.");

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Slug", slug);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                cmsPage = new CmsPage
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Slug = reader.GetString(2),
                    Content = reader.GetString(3),
                    CreatedAt = reader.GetDateTime(4),
                    UpdatedAt = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5)
                };
            }

            if (cmsPage == null)
                return NotFound();

            return Ok(cmsPage);
        }

        // POST: api/CmsPage
        [HttpPost]
        public async Task<ActionResult<CmsPage>> PostCmsPage(CmsPage cmsPage)
        {
            string sql = "INSERT INTO CmsPages (Title, Slug, Content, CreatedAt) OUTPUT INSERTED.Id VALUES (@Title, @Slug, @Content, @CreatedAt)";
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
                return StatusCode(500, "Connection string 'DefaultConnection' not found.");

            cmsPage.CreatedAt = DateTime.UtcNow;

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Title", cmsPage.Title);
            command.Parameters.AddWithValue("@Slug", cmsPage.Slug);
            command.Parameters.AddWithValue("@Content", cmsPage.Content);
            command.Parameters.AddWithValue("@CreatedAt", cmsPage.CreatedAt);

            object? result = await command.ExecuteScalarAsync();
            if (result != null && int.TryParse(result.ToString(), out int newId))
            {
                cmsPage.Id = newId;
                return Ok(cmsPage);
            }

            return StatusCode(500, "Failed to insert CMS page.");
        }
    }

    // ✅ Temporary fallback model (remove if you already have CmsPage.cs in Models folder)
    public class CmsPage
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}