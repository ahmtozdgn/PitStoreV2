using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PitStore.Data;
using PitStore.Models;

namespace PitStore.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetCartAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart> GetOrCreateCartAsync(string userId)
        {
            var cart = await GetCartAsync(userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }
            return cart;
        }

        public async Task<CartItem?> GetCartItemAsync(int cartItemId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId);
        }

        public async Task<IEnumerable<CartItem>> GetCartItemsAsync(string userId)
        {
            var cart = await GetCartAsync(userId);
            if (cart == null)
                return Enumerable.Empty<CartItem>();

            return await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.CartId == cart.Id)
                .ToListAsync();
        }

        public async Task<Cart> AddToCartAsync(string userId, int productId, int quantity)
        {
            var cart = await GetCartAsync(userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<CartItem> UpdateQuantityAsync(int cartItemId, int quantity)
        {
            var cartItem = await GetCartItemAsync(cartItemId);
            if (cartItem == null)
            {
                throw new ArgumentException("Cart item not found.");
            }

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();
            return cartItem;
        }

        public async Task RemoveFromCartAsync(int cartItemId)
        {
            var cartItem = await GetCartItemAsync(cartItemId);
            if (cartItem == null)
            {
                throw new ArgumentException("Cart item not found.");
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetCartTotalAsync(string userId)
        {
            var cart = await GetCartAsync(userId);
            if (cart == null)
            {
                return 0;
            }

            return cart.CartItems.Sum(item =>
            {
                var price = item.Product.IsOnSale && item.Product.SalePrice.HasValue
                    ? item.Product.SalePrice.Value
                    : item.Product.Price;
                return price * item.Quantity;
            });
        }

        public async Task<int> GetCartItemCountAsync(string userId)
        {
            var cart = await GetCartAsync(userId);
            return cart?.CartItems.Sum(ci => ci.Quantity) ?? 0;
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await GetCartAsync(userId);
            if (cart != null)
            {
                cart.CartItems.Clear();
                await _context.SaveChangesAsync();
            }
        }
    }
} 