using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PitStore.Models
{
    public class Cart
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;
        public virtual User? User { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        [NotMapped]
        public decimal TotalAmount => CartItems.Sum(item =>
        {
            var price = item.Product.IsOnSale && item.Product.SalePrice.HasValue
                ? item.Product.SalePrice.Value
                : item.Product.Price;
            return price * item.Quantity;
        });
    }

    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("Cart")]
        public int CartId { get; set; }
        public virtual Cart Cart { get; set; } = null!;

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        [Required]
        [Range(1, 99, ErrorMessage = "Quantity must be between 1 and 99.")]
        public int Quantity { get; set; }

        [NotMapped]
        public decimal TotalPrice => Product.IsOnSale && Product.SalePrice.HasValue
            ? Product.SalePrice.Value * Quantity
            : Product.Price * Quantity;
    }
} 