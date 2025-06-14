using AgriMartAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AgriMartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderStatusController : ControllerBase
    {
        private readonly IOrderStatusRepository _orderStatusRepository;

        public OrderStatusController(IOrderStatusRepository orderStatusRepository)
        {
            _orderStatusRepository = orderStatusRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderStatuses()
        {
            var statuses = await _orderStatusRepository.GetAll();
            return Ok(statuses);
        }
    }
}