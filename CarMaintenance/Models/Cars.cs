using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarMaintenance.Models
{
    public class Cars
    {
        [Key]
        public int CarID { get; set; }

        [Required(ErrorMessage = "Number Plate is required")]
        public string NumberPlate { get; set; }

        // 0 = Unregistered, 1 = Registered
        public int CarStatus { get; set; }

        // ✅ Foreign key → Customer
        public int? CustomerID { get; set; }

        [ForeignKey("CustomerID")]
        [ValidateNever]
        public Customers? Customer { get; set; }

        [ValidateNever]
        public ICollection<Receipts> Receipts { get; set; }

        [ValidateNever]
        public ICollection<TransferCars> TransferCars { get; set; }

        // Computed property
        public string StatusText => CarStatus == 1 ? "Registered" : "Unregistered";
    }
}
