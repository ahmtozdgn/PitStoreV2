using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PitStore.Data;
using PitStore.Models;

namespace PitStore.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPaymentService _paymentService;

        public OrderService(ApplicationDbContext context, IPaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order?> GetOrderByPaymentIntentIdAsync(string paymentIntentId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.PaymentIntentId == paymentIntentId);
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            if (string.IsNullOrEmpty(order.UserId))
            {
                throw new ArgumentException("User ID is required.", nameof(order.UserId));
            }

            if (order.OrderItems == null || !order.OrderItems.Any())
            {
                throw new ArgumentException("Order items are required.", nameof(order.OrderItems));
            }

            if (string.IsNullOrEmpty(order.ShippingAddress))
            {
                throw new ArgumentException("Shipping address is required.", nameof(order.ShippingAddress));
            }

            if (string.IsNullOrEmpty(order.PaymentMethod))
            {
                throw new ArgumentException("Payment method is required.", nameof(order.PaymentMethod));
            }

            order.OrderDate = DateTime.UtcNow;
            order.Status = OrderStatus.Pending;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Process payment
            var paymentResult = await _paymentService.ProcessPaymentAsync(order);
            if (!paymentResult)
            {
                order.Status = OrderStatus.PaymentFailed;
                await _context.SaveChangesAsync();
                throw new Exception("Payment processing failed.");
            }

            order.Status = OrderStatus.Processing;
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return false;
            }

            order.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return false;
            }

            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Processing)
            {
                return false;
            }

            // Restore product quantities
            foreach (var item in order.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                }
            }

            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 