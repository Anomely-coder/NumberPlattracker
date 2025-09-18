using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarMaintenance.Models
{
    public class ReceiptsDetails
    {
        [Key]
        public int ReceiptsDetailsID { get; set; }

        public int ReceiptID { get; set; }

        [ForeignKey("ReceiptID")]
        public Receipts? Receipts { get; set; }

        public int ServiceID { get; set; }

        [ForeignKey("ServiceID")]
        public Services? Services { get; set; }
    }
}
