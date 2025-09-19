using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarMaintenance.Data;
using CarMaintenance.Models;


namespace CarMaintenance.Controllers
{
    public class NumberPlatesController : Controller
    {
        private readonly AppDbContext db;

        public NumberPlatesController(AppDbContext _db)
        {
            db = _db;
        }

        private string GetCurrentUserName()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            return userId != null ? db.Tbl_Users.Find(userId)?.Email ?? "System" : "System";
        }

        public IActionResult Index()
        {
            var plates = db.NumberPlates.ToList();
            return View(plates);
        }

        public async Task<IActionResult> UnAllocatedPlates()
        {
            var plates = await db.NumberPlates
                .Where(p => !p.Allocations.Any())
                .ToListAsync();
            return View(plates);
        }

        public async Task<IActionResult> AllocatedPlates()
        {
            var allocations = await db.PlateAllocations
                .Include(a => a.NumberPlate)
                .GroupBy(a => a.NumberPlateId)
                .Select(g => g.OrderByDescending(a => a.AllocatedOn).First())
                .ToListAsync();

            return View(allocations);
        }

       

        public async Task<IActionResult> Details(int id)
        {
            var plate = await db.NumberPlates
                .Include(p => p.Allocations)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (plate == null) return NotFound();
            return View(plate);
        }

        public IActionResult AddPlate() => View(new NumberPlate());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPlate(NumberPlate numberPlate)
        {
            numberPlate.AddedBy = GetCurrentUserName();
            numberPlate.AddedOn = DateTime.Now;

            if (!ModelState.IsValid) return View(numberPlate);

            db.NumberPlates.Add(numberPlate);
            db.SaveChanges();

            TempData["Message"] = $"Plate {numberPlate.PlateNumber} added successfully.";
            return RedirectToAction(nameof(UnAllocatedPlates));
        }

        public IActionResult EditPlate(int id)
        {
            var plate = db.NumberPlates.Find(id);
            if (plate == null) return NotFound();
            return View(plate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPlate(NumberPlate numberPlate)
        {
            var existing = db.NumberPlates.AsNoTracking().FirstOrDefault(p => p.Id == numberPlate.Id);
            if (existing == null) return NotFound();

            numberPlate.AddedBy = existing.AddedBy;
            numberPlate.AddedOn = existing.AddedOn;
            numberPlate.EditedBy = GetCurrentUserName();
            numberPlate.EditedOn = DateTime.Now;

            if (!ModelState.IsValid) return View(numberPlate);

            db.Update(numberPlate);
            db.SaveChanges();

            TempData["Message"] = $"Plate {numberPlate.PlateNumber} updated successfully.";
            return RedirectToAction(nameof(UnAllocatedPlates));
        }

        [HttpPost]
        public IActionResult AllocatePlate(int plateId, string ownerName)
        {
            var plate = db.NumberPlates.Find(plateId);
            if (plate == null) return NotFound();

            var allocation = new PlateAllocation
            {
                NumberPlateId = plateId,
                OwnerName = ownerName,
                AllocatedBy = GetCurrentUserName(),
                AllocatedOn = DateTime.Now
            };

            db.PlateAllocations.Add(allocation);
            db.SaveChanges();

            TempData["Message"] = $"Plate {plate.PlateNumber} allocated to {ownerName}.";
            return RedirectToAction(nameof(AllocatedPlates));
        }

        [HttpPost]
        public IActionResult TransferPlate(int plateId, string newOwnerName)
        {
            var plate = db.NumberPlates.Find(plateId);
            if (plate == null) return NotFound();

            var allocation = new PlateAllocation
            {
                NumberPlateId = plateId,
                OwnerName = newOwnerName,
                AllocatedBy = GetCurrentUserName(),
                AllocatedOn = DateTime.Now
            };

            db.PlateAllocations.Add(allocation);
            db.SaveChanges();

            TempData["Message"] = $"Plate {plate.PlateNumber} transferred to {newOwnerName}.";
            return RedirectToAction(nameof(AllocatedPlates));
        }

        public IActionResult DeletePlate(int id)
        {
            var plate = db.NumberPlates.Find(id);
            if (plate == null) return NotFound();

            db.PlateAllocations.RemoveRange(db.PlateAllocations.Where(pa => pa.NumberPlateId == id));
            db.NumberPlates.Remove(plate);
            db.SaveChanges();

            TempData["Message"] = $"Plate {plate.PlateNumber} deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
