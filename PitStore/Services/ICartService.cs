using System.Collections.Generic;
using System.Threading.Tasks;
using PitStore.Models;

namespace PitStore.Services
{
    public interface ICartService
    {
        Task<Cart?> GetCartAsync(string userId);
        Task<Cart> GetOrCreateCartAsync(string userId);
        Task<CartItem?> GetCartItemAsync(int cartItemId);
        Task<IEnumerable<CartItem>> GetCartItemsAsync(string userId);
        Task<Cart> AddToCartAsync(string userId, int productId, int quantity);
        Task<CartItem> UpdateQuantityAsync(int cartItemId, int quantity);
        Task RemoveFromCartAsync(int cartItemId);
        Task<decimal> GetCartTotalAsync(string userId);
        Task<int> GetCartItemCountAsync(string userId);
        Task ClearCartAsync(string userId);
    }
} 