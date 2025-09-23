using CarMaintenance.Models;
using System.Collections.Generic;

namespace CarMaintenance.ViewModel
{
    public class CustomerReportViewModel
    {
        public Customers Customer { get; set; }
        public List<Cars> Cars { get; set; }
        public List<TransferCars> Transfers { get; set; }
        public List<Receipts> Receipts { get; set; }
    }
}
