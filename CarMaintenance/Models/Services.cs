using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CarMaintenance.Models
{
    public class Services
    {
        [Key]
        public int ServiceID { get; set; }

        public string ServiceName { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }

        public int ServiceStatus { get; set; }

        // Navigation
        [ValidateNever]
        public ICollection<ReceiptsDetails> ReceiptsDetails { get; set; }

    }
}
