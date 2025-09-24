using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CarMaintenance.Models
{
    public class Cars
    {
        [Key]
        public int CarID { get; set; }

        public string NumberPlate { get; set; }

        // 0 = Unregistered, 1 = Registered
        public int CarStatus { get; set; }

        // Navigation
        [ValidateNever]
        public ICollection<Customers> Customers { get; set; }

        [ValidateNever]
        public ICollection<Receipts> Receipts { get; set; }

        [ValidateNever]
        public ICollection<TransferCars> TransferCars { get; set; }

        // Computed property to show status as text
        public string StatusText => CarStatus == 1 ? "Registered" : "Unregistered";
    }
}
