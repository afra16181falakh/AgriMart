using AgriMartAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgriMartAPI.Repositories
{
    public interface IProductReviewRepository
    {
        Task<IEnumerable<ProductReview>> GetReviewsForProduct(Guid productId);
        Task<ProductReview> CreateReview(ProductReview review);
        Task<bool> DeleteReview(Guid reviewId);
    }
}