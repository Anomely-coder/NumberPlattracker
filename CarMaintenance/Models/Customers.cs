using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarMaintenance.Models
{
    public class Customers
    {
        [Key]
        public int CustomerID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string EmiratesID { get; set; }

        [Required]
        [RegularExpression(@"^05\d{8}$", ErrorMessage = "Mobile number must start with 05 and be 10 digits long.")]
        public string MobileNumber { get; set; }   // 📱 New field

        public int? CarID { get; set; }

        [ForeignKey("CarID")]
        public Cars? Cars { get; set; }

        public int CustomerStatus { get; set; }

        // ✅ New fields for Date & Time
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        [ValidateNever]
        public ICollection<Receipts> Receipts { get; set; }
    }
}
