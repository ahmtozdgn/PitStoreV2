using System.ComponentModel.DataAnnotations;

namespace PitStore.Models
{
    public class Team
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string LogoUrl { get; set; } = string.Empty;

        [StringLength(7)]
        public string? PrimaryColor { get; set; }

        [StringLength(7)]
        public string? SecondaryColor { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Product>? Products { get; set; }
    }
} 