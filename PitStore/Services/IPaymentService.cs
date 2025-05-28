using System.Threading.Tasks;
using PitStore.Models;

namespace PitStore.Services
{
    public interface IPaymentService
    {
        Task<bool> ProcessPaymentAsync(Order order);
        Task<bool> ProcessRefundAsync(Order order);
        Task<bool> ValidatePaymentAsync(string paymentIntentId);
        Task<Payment> ProcessPaymentAsync(int orderId, string userId);
        Task<Payment?> GetPaymentAsync(int id);
    }
} 