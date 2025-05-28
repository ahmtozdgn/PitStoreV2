using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PitStore.Data;
using PitStore.ViewModels;
using PitStore.Models;

namespace PitStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var viewModel = new AdminDashboardViewModel
            {
                TotalUsers = _context.Users.Count(),
                TotalProducts = _context.Products.Count(),
                TotalOrders = _context.Orders.Count(),
                TotalRevenue = _context.Orders.Sum(o => o.TotalAmount),
                RecentOrders = _context.Orders
                    .Include(o => o.User)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToList(),
                LowStockProducts = _context.Products
                    .Where(p => p.StockQuantity < 10)
                    .ToList()
            };

            return View(viewModel);
        }

        public IActionResult Products()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        public IActionResult Orders()
        {
            var orders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        public IActionResult Users()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderId, OrderStatus newStatus)
        {
            var order = _context.Orders.Find(orderId);
            if (order != null)
            {
                order.Status = newStatus;
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Orders));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.IsAdmin)
            {
                TempData["ErrorMessage"] = "Admin kullanıcılar silinemez.";
                return RedirectToAction(nameof(Users));
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Kullanıcı başarıyla silindi.";
            return RedirectToAction(nameof(Users));
        }

        public IActionResult Dashboard()
        {
            var viewModel = new AdminDashboardViewModel
            {
                TotalUsers = _context.Users.Count(),
                TotalProducts = _context.Products.Count(),
                TotalOrders = _context.Orders.Count(),
                TotalRevenue = _context.Orders.Sum(o => o.TotalAmount),
                RecentOrders = _context.Orders
                    .Include(o => o.User)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToList(),
                LowStockProducts = _context.Products
                    .Where(p => p.StockQuantity < 10)
                    .ToList()
            };

            return View(viewModel);
        }
    }
} 