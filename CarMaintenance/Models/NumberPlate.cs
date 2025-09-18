using System;
using System.Collections.Generic;

namespace CarMaintenance.Models
{
    public class NumberPlate
    {
        public int Id { get; set; }
        public string PlateNumber { get; set; }
        public string AddedBy { get; set; }
        public DateTime? AddedOn { get; set; }

        public ICollection<PlateAllocation> Allocations { get; set; }
    }
}
