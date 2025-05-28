using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PitStore.Data;
using PitStore.Models;
using PitStore.Models.ViewModels;
using PitStore.Services;
using System.Security.Claims;

namespace PitStore.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly ApplicationDbContext _context;

        public CartController(ICartService cartService, IProductService productService, ApplicationDbContext context)
        {
            _cartService = cartService;
            _productService = productService;
            _context = context;
        }

        public async Task<IActionResult> Index(decimal totalPrice)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = await _cartService.GetCartAsync(userId);
            if (cart == null)
            {
                return View(new List<CartItemViewModel>());
            }

            var cartItems = new List<CartItemViewModel>();
            foreach (var item in cart.CartItems)
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                if (product != null)
                {
                    cartItems.Add(new CartItemViewModel
                    {
                        Id = item.Id,
                        ProductId = product.Id,
                        ProductName = product.Name,
                        ProductImageUrl = product.ImageUrl,
                        UnitPrice = product.Price,
                        Quantity = item.Quantity
                    });
                }
            }

            ViewBag.CartTotal = cart.TotalAmount.ToString() ?? "0";
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Please login to add items to cart." });
            }

            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }

            if (product.StockQuantity < quantity)
            {
                return Json(new { success = false, message = "Not enough stock available." });
            }

            await _cartService.AddToCartAsync(userId, productId, quantity);
            var cartTotal = await _cartService.GetCartTotalAsync(userId);
            var cartCount = await _cartService.GetCartItemCountAsync(userId);

            return Json(new { success = true, cartTotal = cartTotal, cartCount = cartCount });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Please login to update cart." });
            }

            var cartItem = await _cartService.GetCartItemAsync(cartItemId);
            if (cartItem == null || cartItem.Cart.UserId != userId)
            {
                return Json(new { success = false, message = "Cart item not found." });
            }

            var product = await _productService.GetProductByIdAsync(cartItem.ProductId);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }

            if (product.StockQuantity < quantity)
            {
                return Json(new { success = false, message = "Not enough stock available." });
            }

            await _cartService.UpdateQuantityAsync(cartItemId, quantity);
            var cartTotal = await _cartService.GetCartTotalAsync(userId);

            return Json(new { success = true, cartTotal = cartTotal });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Please login to remove items from cart." });
            }

            var cartItem = await _cartService.GetCartItemAsync(cartItemId);
            if (cartItem == null || cartItem.Cart == null || cartItem.Product == null || cartItem.Cart.UserId != userId)
            {
                return Json(new { success = false, message = "Cart item not found." });
            }

            await _cartService.RemoveFromCartAsync(cartItemId);
            var cartTotal = await _cartService.GetCartTotalAsync(userId);

            return Json(new { success = true, cartTotal = cartTotal });
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Lütfen giriş yapın." });
            }

            try
            {
                await _cartService.ClearCartAsync(userId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { count = 0 });
            }

            var cart = await _cartService.GetCartAsync(userId);
            var count = cart?.CartItems?.Sum(ci => ci.Quantity) ?? 0;
            
            return Json(new { count = count });
        }

        [HttpGet]
        public async Task<IActionResult> GetCartPreview()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return PartialView("_CartPreview", new List<CartItem>());
            }

            var cart = await _cartService.GetCartAsync(userId);
            var cartItems = cart?.CartItems ?? new List<CartItem>();
            
            return PartialView("_CartPreview", cartItems);
        }

        [HttpGet]
        public async Task<IActionResult> GetCartTotal()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(0);
            }

            var cartTotal = await _cartService.GetCartTotalAsync(userId);
            return Json(cartTotal);
        }

        public IActionResult Checkout()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(string shippingAddress, string paymentMethod)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            var cart = await _cartService.GetCartAsync(userId);
            if (cart == null || !cart.CartItems.Any())
            {
                ModelState.AddModelError("", "Sepetiniz boş.");
                return View();
            }
            var order = new Order
            {
                UserId = userId,
                ShippingAddress = shippingAddress,
                PaymentMethod = paymentMethod,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product.Price
                }).ToList(),
                OrderDate = DateTime.UtcNow,
                TotalAmount = cart.TotalAmount
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            await _cartService.ClearCartAsync(userId);
            return RedirectToAction("ThankYou");
        }

        public IActionResult ThankYou()
        {
            return View();
        }
    }
} 