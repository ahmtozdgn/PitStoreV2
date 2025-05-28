using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace PitStore.Models
{
    public class Product
    {
        public Product()
        {
            Reviews = new List<Review>();
            CreatedAt = DateTime.UtcNow;
        }

        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(200)]
        public string? TeamLogoUrl { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? OriginalPrice { get; set; }

        public int StockQuantity { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Foreign keys
        public int? TeamId { get; set; }
        public int? CategoryId { get; set; }

        // Navigation properties
        public virtual Team? Team { get; set; }
        public virtual Category? Category { get; set; }

        // Technical specifications
        [StringLength(500)]
        public string? Specifications { get; set; }

        [StringLength(100)]
        public string? Material { get; set; }

        [StringLength(100)]
        public string? Size { get; set; }

        [StringLength(100)]
        public string? Color { get; set; }

        // F1 specific properties
        [StringLength(100)]
        public string? DriverName { get; set; }

        [StringLength(100)]
        public string? Season { get; set; }

        public bool IsLimitedEdition { get; set; }

        public bool IsSigned { get; set; }

        // Shipping information
        public bool IsInStock { get; set; } = true;

        public bool IsShipped { get; set; }

        public int? EstimatedDeliveryDays { get; set; }

        // SEO properties
        [StringLength(200)]
        public string? MetaTitle { get; set; }

        [StringLength(500)]
        public string? MetaDescription { get; set; }

        [StringLength(200)]
        public string? Slug { get; set; }

        // Ratings and reviews
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        // Team colors for UI customization
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public string? TertiaryColor { get; set; }

        // Additional images
        public string? AdditionalImage1Url { get; set; }
        public string? AdditionalImage2Url { get; set; }
        public string? AdditionalImage3Url { get; set; }

        // Product status
        public bool IsOnSale { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Sale price must be greater than or equal to 0.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SalePrice { get; set; }
        public DateTime? SaleStartDate { get; set; }
        public DateTime? SaleEndDate { get; set; }

        [NotMapped]
        public decimal CurrentPrice => IsOnSale && SalePrice.HasValue ? SalePrice.Value : Price;

        [NotMapped]
        public decimal AverageRating => Reviews?.Any() == true ? (decimal)Reviews.Average(r => r.Rating) : 0;

        [NotMapped]
        public int ReviewCount => Reviews?.Count ?? 0;

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        [NotMapped]
        public IFormFile? AdditionalImage1File { get; set; }

        [NotMapped]
        public IFormFile? AdditionalImage2File { get; set; }

        [NotMapped]
        public IFormFile? AdditionalImage3File { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    }
} 