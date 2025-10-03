using CarMaintenance.Data;
using CarMaintenance.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CarMaintenance.Controllers
{
    public class CarsController : Controller
    {
        private readonly AppDbContext db;

        public CarsController(AppDbContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            var cars = db.Tbl_Cars.Include(c => c.Customers).ToList();
            return View(cars);
        }

        public IActionResult AddCar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCar(Cars cars)
        {
            // Check if number plate already exists
            if (db.Tbl_Cars.Any(c => c.NumberPlate == cars.NumberPlate))
            {
                ModelState.AddModelError("NumberPlate", "This number plate already exists.");
            }

            if (ModelState.IsValid)
            {
                db.Tbl_Cars.Add(cars);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cars);
        }

        public IActionResult EditCar(int Id)
        {
            var data = db.Tbl_Cars.Find(Id);
            return View(data);
        }

        [HttpPost]
        public IActionResult EditCar(Cars cars)
        {
            // Check if number plate already exists (excluding same car)
            if (db.Tbl_Cars.Any(c => c.NumberPlate == cars.NumberPlate && c.CarID != cars.CarID))
            {
                ModelState.AddModelError("NumberPlate", "This number plate already exists.");
            }

            if (ModelState.IsValid)
            {
                db.Tbl_Cars.Update(cars);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cars);
        }

        public IActionResult DeleteCar(int Id)
        {
            var data = db.Tbl_Cars.Find(Id);

            if (data != null)
            {
                try
                {
                    db.Tbl_Cars.Remove(data);
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Car deleted successfully!";
                }
                catch (DbUpdateException)
                {
                    // 🔒 Car is linked to Receipts or another table → show blocking message
                    TempData["ErrorMessage"] = "This car cannot be deleted because it has receipts linked to it.";
                }
                catch (Exception ex)
                {
                    // fallback for unexpected errors
                    TempData["ErrorMessage"] = "An unexpected error occurred: " + ex.Message;
                }
            }

            return RedirectToAction("Index");
        }
    }
}
