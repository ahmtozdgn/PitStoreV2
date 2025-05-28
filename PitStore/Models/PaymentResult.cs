namespace PitStore.Models
{
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string? TransactionId { get; set; }
        public string? Message { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
        public DateTime? ProcessedAt { get; set; } = DateTime.UtcNow;
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
    }
} 