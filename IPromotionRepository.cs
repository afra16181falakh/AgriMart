using AgriMartAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgriMartAPI.Repositories
{
    public interface IPromotionRepository
    {
        Task<IEnumerable<Promotion>> GetActivePromotions();
        // You would add more for admin (Create, Update, Delete)
    }
}