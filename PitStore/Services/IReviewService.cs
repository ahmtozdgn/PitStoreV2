using System.Collections.Generic;
using System.Threading.Tasks;
using PitStore.Models;

namespace PitStore.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<Review>> GetProductReviewsAsync(int productId);
        Task<Review?> GetReviewByIdAsync(int id);
        Task<Review> CreateReviewAsync(Review review);
        Task<Review?> UpdateReviewAsync(int id, Review review);
        Task<bool> DeleteReviewAsync(int id);
        Task<double> GetAverageRatingAsync(int productId);
        Task<bool> HasUserReviewedProductAsync(string userId, int productId);
    }
} 