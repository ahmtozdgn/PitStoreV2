using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PitStore.Models
{
    public class Order
    {
        public Order()
        {
            OrderItems = new List<OrderItem>();
            OrderStatus = OrderStatus.Pending;
        }

        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;
        public virtual User? User { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Shipping address cannot be longer than 500 characters.")]
        public string ShippingAddress { get; set; } = string.Empty;
        public string? BillingAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Total amount must be greater than or equal to 0.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public string? PaymentIntentId { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        [Required]
        [StringLength(50, ErrorMessage = "Payment method cannot be longer than 50 characters.")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required]
        public OrderStatus OrderStatus { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        [NotMapped]
        public int ItemCount => OrderItems.Sum(item => item.Quantity);

        public OrderStatus Status { get; internal set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        [Required]
        [Range(1, 99, ErrorMessage = "Quantity must be between 1 and 99.")]
        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Unit price must be greater than or equal to 0.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SalePrice { get; set; }
        public bool IsOnSale { get; set; }

        [NotMapped]
        public decimal TotalPrice => (IsOnSale && SalePrice.HasValue ? SalePrice.Value : UnitPrice) * Quantity;
    }
} 