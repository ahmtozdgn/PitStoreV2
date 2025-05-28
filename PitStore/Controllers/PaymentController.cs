using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PitStore.Data;
using PitStore.Models;
using PitStore.Services;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PitStore.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly ILogger<PaymentController> _logger;

        public Payment? paymentIntent { get; private set; }

        public PaymentController(ApplicationDbContext context, IPaymentService paymentService, IOrderService orderService, ILogger<PaymentController> logger)
        {
            _context = context;
            _paymentService = paymentService;
            _orderService = orderService;
            _logger = logger;
        }

        public IActionResult Index(decimal amount)
        {
            if (amount <= 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            var payment = new Payment
            {
                Amount = amount
            };

            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(Payment payment)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", payment);
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                PaymentMethod = "Credit Card",
                TotalAmount = cart.CartItems.Sum(ci => ci.Product.IsOnSale ? (ci.Product.SalePrice ?? ci.Product.Price) * ci.Quantity : ci.Product.Price * ci.Quantity),
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product.Price,
                    IsOnSale = ci.Product.IsOnSale,
                    SalePrice = ci.Product.SalePrice
                }).ToList()
            };

            var success = await _paymentService.ProcessPaymentAsync(order);

            if (success)
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();

                return RedirectToAction("Confirmation", "Order", new { id = order.Id });
            }

            ModelState.AddModelError("", "Ödeme işlemi başarısız oldu. Lütfen kart bilgilerinizi kontrol edin.");
            return View("Index", payment);
        }

        public Payment? GetPaymentIntent()
        {
            return paymentIntent;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(string paymentIntentId)
        {
            try
            {
                var order = await _orderService.GetOrderByPaymentIntentIdAsync(paymentIntentId);
                if (order == null)
                {
                    return NotFound();
                }

                var result = await _paymentService.ProcessPaymentAsync(order);
                if (result)
                {
                    return RedirectToAction("Success", new { orderId = order.Id });
                }

                return RedirectToAction("Error", new { message = "Payment processing failed." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment");
                return RedirectToAction("Error", new { message = "An error occurred while processing your payment." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Refund(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            if (order.UserId != User.Identity?.Name)
            {
                return Forbid();
            }

            var result = await _paymentService.ProcessRefundAsync(order);
            if (result)
            {
                await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.Refunded);
                return RedirectToAction("Details", "Order", new { id = orderId });
            }

            return RedirectToAction("Details", "Order", new { id = orderId, error = "Refund failed" });
        }

        public async Task<IActionResult> Success(int paymentId)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = User.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var payment = await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.Id == paymentId && p.Order.UserId == userId);

            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        public async Task<IActionResult> Failed(int paymentId)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = User.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var payment = await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.Id == paymentId && p.Order.UserId == userId);

            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }
    }
} 