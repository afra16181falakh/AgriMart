using AgriMartAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgriMartAPI.Repositories
{
    public interface IOrderStatusRepository
    {
        Task<IEnumerable<OrderStatus>> GetAll();
    }
}