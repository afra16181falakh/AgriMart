using AgriMartAPI.Models;
using AgriMartAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AgriMartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileRepository _userProfileRepository;

        public UserProfileController(IUserProfileRepository userProfileRepository)
        {
            _userProfileRepository = userProfileRepository;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserProfile(Guid userId)
        {
            var profile = await _userProfileRepository.GetUserProfile(userId);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUserProfile(Guid userId, [FromBody] UserProfile userProfile)
        {
            if (userId != userProfile.UserId) return BadRequest("ID mismatch.");
            
            var success = await _userProfileRepository.UpdateUserProfile(userProfile);
            if (!success) return NotFound();
            
            return NoContent();
        }
    }
}