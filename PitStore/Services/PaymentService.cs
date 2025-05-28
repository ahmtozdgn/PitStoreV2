using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PitStore.Data;
using PitStore.Models;

namespace PitStore.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;

        public PaymentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ProcessPaymentAsync(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            try
            {
                // Simulate payment processing
                await Task.Delay(1000); // Simulate API call

                order.PaymentStatus = PaymentStatus.Succeeded;
                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                order.PaymentStatus = PaymentStatus.Failed;
                await _context.SaveChangesAsync();
                return false;
            }
        }

        public async Task<bool> ValidatePaymentAsync(string paymentIntentId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.PaymentIntentId == paymentIntentId);

            return order != null && order.PaymentStatus == PaymentStatus.Succeeded;
        }

        public async Task<bool> ProcessRefundAsync(Order order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            if (order.PaymentStatus != PaymentStatus.Succeeded)
            {
                throw new InvalidOperationException("Cannot refund an order that hasn't been paid.");
            }

            // Simulate refund process
            await Task.Delay(1000); // Simulate API call

            order.PaymentStatus = PaymentStatus.Refunded;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Payment?> ProcessPaymentAsync(int orderId, string userId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
                return null;

            var payment = new Payment
            {
                OrderId = orderId,
                Amount = order.TotalAmount,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return payment;
        }

        public async Task<Payment?> GetPaymentAsync(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.Id == id);

            return payment;
        }

        public async Task<PaymentResult?> ProcessPaymentAsync(PaymentRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            try
            {
                // Validate the order exists
                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.UserId == request.UserId);

                if (order == null)
                {
                    return new PaymentResult
                    {
                        Success = false,
                        Message = "Order not found",
                        ErrorCode = "ORDER_NOT_FOUND"
                    };
                }

                // Simulate payment processing
                await Task.Delay(1000); // Simulate API call

                // Create payment record
                var payment = new Payment
                {
                    OrderId = request.OrderId,
                    Amount = request.Amount,
                    Status = PaymentStatus.Succeeded,
                    CreatedAt = DateTime.UtcNow,
                    TransactionId = Guid.NewGuid().ToString()
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                return new PaymentResult
                {
                    Success = true,
                    TransactionId = payment.TransactionId,
                    Message = "Payment processed successfully",
                    Amount = request.Amount,
                    Currency = request.Currency,
                    ProcessedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return new PaymentResult
                {
                    Success = false,
                    Message = "Payment processing failed",
                    ErrorCode = "PAYMENT_FAILED",
                    ErrorMessage = ex.Message
                };
            }
        }
    }
} 