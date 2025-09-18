using System;

namespace CarMaintenance.Models
{
    public class PlateAllocation
    {
        public int Id { get; set; }
        public int NumberPlateId { get; set; }
        public string OwnerName { get; set; }
        public string AllocatedBy { get; set; }
        public DateTime AllocatedOn { get; set; }
        public string EditedBy { get; set; }
        public DateTime? EditedOn { get; set; }

        public NumberPlate NumberPlate { get; set; }
    }
}
