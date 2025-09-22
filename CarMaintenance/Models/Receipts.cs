using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarMaintenance.Models
{
    public class Receipts
    {
        [Key]
        public int ReceiptID { get; set; }

        public int CustomerID { get; set; }

        [ForeignKey("CustomerID")]
        public Customers? Customers { get; set; }

        public int CarID { get; set; }

        [ForeignKey("CarID")]
        public Cars? Cars { get; set; }

        public DateTime Date { get; set; }

        public decimal TotalAmount { get; set; }

        [ValidateNever]
        public ICollection<ReceiptsDetails> ReceiptsDetails { get; set; } = new List<ReceiptsDetails>();
    }
}