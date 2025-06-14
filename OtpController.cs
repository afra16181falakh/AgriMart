using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System;

namespace AgriMartAPI.Controllers
{
    public class GenerateOtpRequest
    {
        public string Identifier { get; set; } = string.Empty;  // Prevent null warnings
    }

    public class VerifyOtpRequest
    {
        public string Identifier { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    [Route("api/[controller]")]
    [ApiController]
    public class OtpController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public OtpController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateOtp([FromBody] GenerateOtpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Identifier))
                return BadRequest("Identifier is required.");

            string otpCode = new Random().Next(100000, 999999).ToString("D6");
            DateTime expiryTime = DateTime.UtcNow.AddMinutes(5);

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
                return StatusCode(500, "Connection string is missing.");

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            string deleteSql = "DELETE FROM Otps WHERE Identifier = @Identifier";
            await using (var deleteCmd = new SqlCommand(deleteSql, connection))
            {
                deleteCmd.Parameters.AddWithValue("@Identifier", request.Identifier);
                await deleteCmd.ExecuteNonQueryAsync();
            }

            string insertSql = "INSERT INTO Otps (Identifier, Code, ExpiryTime) VALUES (@Identifier, @Code, @ExpiryTime)";
            await using (var insertCmd = new SqlCommand(insertSql, connection))
            {
                insertCmd.Parameters.AddWithValue("@Identifier", request.Identifier);
                insertCmd.Parameters.AddWithValue("@Code", otpCode);
                insertCmd.Parameters.AddWithValue("@ExpiryTime", expiryTime);
                await insertCmd.ExecuteNonQueryAsync();
            }

            return Ok(new { message = "OTP has been generated successfully." });
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Identifier) || string.IsNullOrWhiteSpace(request.Code))
                return BadRequest("Identifier and code are required.");

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
                return StatusCode(500, "Connection string is missing.");

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            string selectSql = "SELECT Code, ExpiryTime FROM Otps WHERE Identifier = @Identifier";
            string? dbCode = null;
            DateTime? dbExpiry = null;

            await using (var selectCmd = new SqlCommand(selectSql, connection))
            {
                selectCmd.Parameters.AddWithValue("@Identifier", request.Identifier);
                await using var reader = await selectCmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    dbCode = reader.GetString(0);
                    dbExpiry = reader.GetDateTime(1);
                }
            }

            if (dbCode == null || dbExpiry == null)
                return BadRequest(new { isValid = false, message = "Invalid identifier or OTP." });

            if (dbExpiry < DateTime.UtcNow)
                return BadRequest(new { isValid = false, message = "OTP has expired." });

            if (dbCode != request.Code)
                return BadRequest(new { isValid = false, message = "Incorrect OTP." });

            string deleteSql = "DELETE FROM Otps WHERE Identifier = @Identifier";
            await using (var deleteCmd = new SqlCommand(deleteSql, connection))
            {
                deleteCmd.Parameters.AddWithValue("@Identifier", request.Identifier);
                await deleteCmd.ExecuteNonQueryAsync();
            }

            return Ok(new { isValid = true, message = "OTP verified successfully." });
        }
    }
}