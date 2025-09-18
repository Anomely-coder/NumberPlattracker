using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarMaintenance.Data;
using CarMaintenance.Models;   // make sure this is included
using System.Threading.Tasks;
using System.Linq;
using System;

namespace CarMaintenance.Controllers
{
    public class NumberPlatesController : Controller
    {
        private readonly AppDbContext _context;

        public NumberPlatesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> UnAllocatedPlates()
        {
            var plates = await _context.NumberPlates
                .Where(p => !_context.PlateAllocations.Any(a => a.NumberPlateId == p.Id))
                .ToListAsync();
            return View(plates);
        }

        public async Task<IActionResult> AllocatedPlates()
        {
            var plates = await _context.PlateAllocations
                .Include(a => a.NumberPlate)
                .ToListAsync();
            return View(plates);
        }

        public async Task<IActionResult> PlateHistory()
        {
            var history = await _context.PlateAllocations
                .Include(a => a.NumberPlate)
                .OrderByDescending(a => a.AllocatedOn)
                .ToListAsync();
            return View(history);
        }

        // 🔹 New Method to Allocate a Plate
        [HttpPost]
        public async Task<IActionResult> AllocatePlate(int plateId, string ownerName)
        {
            var allocation = new PlateAllocation
            {
                NumberPlateId = plateId,
                OwnerName = ownerName,
                AllocatedBy = User.Identity?.Name ?? "System", // fallback if user not logged in
                AllocatedOn = DateTime.Now
            };

            _context.PlateAllocations.Add(allocation);
            await _context.SaveChangesAsync();

            return RedirectToAction("AllocatedPlates");
        }
    }
}
