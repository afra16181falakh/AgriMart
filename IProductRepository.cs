using AgriMartAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgriMartAPI.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProducts();
        Task<Product?> GetProductById(Guid id); // The ? makes the return type nullable
        Task<Product> CreateProduct(Product product);
        Task<bool> UpdateProduct(Product product); // Return bool for success/failure
        Task<bool> DeleteProduct(Guid id);
    }
}