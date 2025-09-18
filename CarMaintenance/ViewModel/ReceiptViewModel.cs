using CarMaintenance.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CarMaintenance.ViewModel
{
    public class ReceiptViewModel
    {
        [Required]
        public int CustomerID { get; set; }

        public string? NumberPlate { get; set; }

        public int CarID { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }

        public List<ReceiptServiceItem> ServicesSelected { get; set; } = new();

        public SelectList? CustomersList { get; set; }

        public SelectList? ServicesList { get; set; }
    }

    public class ReceiptServiceItem
    {
        public int ServiceID { get; set; }
        public string? ServiceName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }
}
