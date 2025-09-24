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
        [StringLength(15, MinimumLength = 15, ErrorMessage = "Emirates ID must be exactly 15 characters long.")]
        public string EmiratesID { get; set; }

        public int? CarID { get; set; }

        [ForeignKey("CarID")]
        public Cars? Cars { get; set; }

        public int CustomerStatus { get; set; }

        [ValidateNever]
        public ICollection<Receipts> Receipts { get; set; }
    }
}