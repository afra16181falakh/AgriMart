using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using AgriMartAPI.Models;

namespace AgriMartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public NotificationsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/notifications/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotificationsForUser(Guid userId)
        {
            string connectionString = _configuration.GetConnectionString("InputShopConnection")!;
            var notifications = new List<Notification>();

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string sql = "SELECT * FROM Notification WHERE UserId = @UserId ORDER BY CreatedDate DESC";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            notifications.Add(MapReaderToNotification(reader));
                        }
                    }
                }
            }

            return Ok(notifications);
        }

        // POST: api/notifications
        [HttpPost]
        public async Task<ActionResult<Notification>> CreateNotification([FromBody] Notification notification)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            notification.Id = Guid.NewGuid();
            notification.CreatedDate = DateTime.UtcNow;
            notification.IsRead = false;

            string connectionString = _configuration.GetConnectionString("InputShopConnection")!;
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string sql = @"INSERT INTO Notification 
                               (Id, UserId, Title, Message, IsRead, NotificationType, LinkUrl, CreatedDate)
                               VALUES 
                               (@Id, @UserId, @Title, @Message, @IsRead, @NotificationType, @LinkUrl, @CreatedDate)";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", notification.Id);
                    command.Parameters.AddWithValue("@UserId", notification.UserId);
                    command.Parameters.AddWithValue("@Title", notification.Title);
                    command.Parameters.AddWithValue("@Message", notification.Message);
                    command.Parameters.AddWithValue("@IsRead", notification.IsRead);
                    command.Parameters.AddWithValue("@NotificationType", (object?)notification.NotificationType ?? DBNull.Value);
                    command.Parameters.AddWithValue("@LinkUrl", (object?)notification.LinkUrl ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedDate", notification.CreatedDate);

                    await command.ExecuteNonQueryAsync();
                }
            }

            return Ok(notification);
        }

        // PUT: api/notifications/{notificationId}/read
        [HttpPut("{notificationId}/read")]
        public async Task<IActionResult> MarkAsRead(Guid notificationId)
        {
            string connectionString = _configuration.GetConnectionString("InputShopConnection")!;
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string sql = "UPDATE Notification SET IsRead = 1 WHERE Id = @Id";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", notificationId);
                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                        return NotFound();
                }
            }

            return NoContent();
        }

        // Helper method to map reader to Notification model
        private Notification MapReaderToNotification(SqlDataReader reader)
        {
            return new Notification
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Message = reader.GetString(reader.GetOrdinal("Message")),
                IsRead = reader.GetBoolean(reader.GetOrdinal("IsRead")),
                NotificationType = reader.IsDBNull(reader.GetOrdinal("NotificationType")) ? null : reader.GetString(reader.GetOrdinal("NotificationType")),
                LinkUrl = reader.IsDBNull(reader.GetOrdinal("LinkUrl")) ? null : reader.GetString(reader.GetOrdinal("LinkUrl")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
            };
        }
    }
}