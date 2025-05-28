using System.Collections.Generic;
using System.Threading.Tasks;
using PitStore.Models;

namespace PitStore.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetProductsByTeamAsync(int teamId);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<Product> CreateProductAsync(Product product);
        Task<Product?> UpdateProductAsync(int id, Product product);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> UpdateStockAsync(int id, int quantity);
    }
} 