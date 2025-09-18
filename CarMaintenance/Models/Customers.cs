using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarMaintenance.Models
{
    public class Customers
    {
        [Key]
        public int CustomerID { get; set; }

        public string Name { get; set; }

        public string CNIC { get; set; }

        public int? CarID { get; set; }

        [ForeignKey("CarID")]
        public Cars? Cars { get; set; }

        public int CustomerStatus { get; set; }

        // Navigation
        [ValidateNever]
        public ICollection<Receipts> Receipts { get; set; }

    }
}
