using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarMaintenance.Models
{
    public class TransferCars
    {
        [Key]
        public int TransferID { get; set; }

        public DateTime TransferDate { get; set; } = DateTime.Now;

        public int FromCustomerID { get; set; }

        [ForeignKey("FromCustomerID")]
        public Customers? FromCustomers { get; set; }

        public int ToCustomerID { get; set; }

        [ForeignKey("ToCustomerID")]
        public Customers? ToCustomers { get; set; }

        public int CarID { get; set; }

        [ForeignKey("CarID")]
        public Cars? Cars { get; set; }

    }
}
