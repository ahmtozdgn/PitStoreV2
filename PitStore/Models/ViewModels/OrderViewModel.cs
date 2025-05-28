using System.ComponentModel.DataAnnotations;

namespace PitStore.Models.ViewModels
{
    public class OrderViewModel
    {
        [Required(ErrorMessage = "Shipping address is required.")]
        [StringLength(500, ErrorMessage = "Shipping address cannot be longer than 500 characters.")]
        public string ShippingAddress { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Billing address cannot be longer than 500 characters.")]
        public string? BillingAddress { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;
    }
} 