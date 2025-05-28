using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PitStore.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CardHolderName { get; set; } = string.Empty;

        [Required]
        [CreditCard]
        [StringLength(19)]
        public string CardNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(5)]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", ErrorMessage = "Geçerli bir son kullanma tarihi giriniz (MM/YY)")]
        public string ExpiryDate { get; set; } = string.Empty;

        [Required]
        [StringLength(3)]
        [RegularExpression(@"^[0-9]{3}$", ErrorMessage = "Geçerli bir CVC kodu giriniz")]
        public string CVC { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Tutar 0'dan büyük olmalıdır")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string? TransactionId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

    public enum PaymentStatus
    {
        Pending,
        Succeeded,
        Failed,
        Refunded
    }
} 