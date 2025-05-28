using System.ComponentModel.DataAnnotations;

namespace PitStore.Models
{
    public class PaymentRequest
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [StringLength(100)]
        public string? CardNumber { get; set; }

        [StringLength(50)]
        public string? CardHolderName { get; set; }

        [StringLength(5)]
        public string? ExpiryDate { get; set; }

        [StringLength(4)]
        public string? CVV { get; set; }

        [StringLength(200)]
        public string? BillingAddress { get; set; }

        [StringLength(100)]
        public string? Currency { get; set; } = "TRY";

        public string? UserId { get; set; }
    }
} 