using System.Collections.Generic;
using System.Threading.Tasks;
using PitStore.Models;

namespace PitStore.Services
{
    public interface IOrderService
    {
        Task<Order?> GetOrderByIdAsync(int id);
        Task<Order?> GetOrderByPaymentIntentIdAsync(string paymentIntentId);
        Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
        Task<Order> CreateOrderAsync(Order order);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task<bool> CancelOrderAsync(int orderId);
    }
} 