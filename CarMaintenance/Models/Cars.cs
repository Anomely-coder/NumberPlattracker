using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CarMaintenance.Models
{
    public class Cars
    {
        [Key]
        public int CarID { get; set; }

        public string CarName { get; set; }
        public string NumberPlate { get; set; }
        public string CarModel { get; set; }

        public int CarStatus { get; set; }


        // Navigation
        [ValidateNever]
        public ICollection<Customers> Customers { get; set; }

        [ValidateNever]
        public ICollection<Receipts> Receipts { get; set; }

        [ValidateNever]
        public ICollection<TransferCars> TransferCars { get; set; }
    }
}
