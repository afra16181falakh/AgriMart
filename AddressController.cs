using AgriMartAPI.Models;
using AgriMartAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AgriMartAPI.Controllers
{
    [Route("api/Address")] // This correctly sets your API route to singular
    [ApiController]
    public class AddressController : ControllerBase // Class name is singular
    {
        private readonly IAddressRepository _addressRepository;

        // CORRECTED LINE: Constructor name must match the class name
        public AddressController(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAddress(Guid userId)
        {
            // Assuming _addressRepository.GetAddressByUserId already exists or you will create it
            return Ok(await _addressRepository.GetAddressByUserId(userId));
        }

        // ... all other endpoints refactored to call the repository ...
    }
}