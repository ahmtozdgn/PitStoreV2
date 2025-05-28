using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PitStore.Data;
using PitStore.Models;
using PitStore.Models.ViewModels;
using PitStore.Services;
using System.Threading.Tasks;

namespace PitStore.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;

        public OrderController(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = await _orderService.GetUserOrdersAsync(userId);
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            if (order.UserId != User.Identity?.Name)
            {
                return Forbid();
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = await _cartService.GetCartItemsAsync(userId);
            if (!cartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var order = new Order
            {
                UserId = userId,
                ShippingAddress = model.ShippingAddress,
                BillingAddress = model.BillingAddress,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                PaymentMethod = "Credit Card", // Default payment method
                OrderItems = cartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product.Price,
                    IsOnSale = ci.Product.IsOnSale,
                    SalePrice = ci.Product.SalePrice
                }).ToList()
            };

            await _orderService.CreateOrderAsync(order);
            await _cartService.ClearCartAsync(userId);

            return RedirectToAction("Details", new { id = order.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            if (order.UserId != User.Identity?.Name)
            {
                return Forbid();
            }

            var result = await _orderService.CancelOrderAsync(id);
            if (result)
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            return RedirectToAction(nameof(Details), new { id, error = "Sipariş iptal edilemedi" });
        }

        public IActionResult Success(int id)
        {
            return View(id);
        }

        public IActionResult Failed()
        {
            return View();
        }
    }
} 