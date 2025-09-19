using System;
using System.ComponentModel.DataAnnotations;

namespace CarMaintenance.Models
{
    public class PlateAllocation
    {
        public int Id { get; set; }

        [Required]
        public int NumberPlateId { get; set; }
        public NumberPlate NumberPlate { get; set; }

        [Required]
        public string OwnerName { get; set; }

        [Required]
        public string AllocatedBy { get; set; }

        [Required]
        public DateTime AllocatedOn { get; set; }
    }
}
