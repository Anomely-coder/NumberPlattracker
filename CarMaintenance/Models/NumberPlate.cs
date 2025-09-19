using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CarMaintenance.Models
{
    public class NumberPlate
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression("^[A-Za-z0-9]+$", ErrorMessage = "Plate number must be alpha-numeric")]
        public string PlateNumber { get; set; }

        public string AddedBy { get; set; }
        public DateTime? AddedOn { get; set; }

        public string EditedBy { get; set; }
        public DateTime? EditedOn { get; set; }

        public ICollection<PlateAllocation> Allocations { get; set; } = new List<PlateAllocation>();
    }
}
