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

        // ✅ Show all cars with their related customer
        public IActionResult Index()
        {
            var cars = db.Tbl_Cars
                         .Include(c => c.Customer) // each car belongs to one customer
                         .ToList();
            return View(cars);
        }

        // ✅ Add new car (GET)
        public IActionResult AddCar()
        {
            ViewBag.Customers = db.Tbl_Customers.ToList(); // for dropdown selection
            return View();
        }

        // ✅ Add new car (POST)
        [HttpPost]
        public IActionResult AddCar(Cars cars)
        {
            if (db.Tbl_Cars.Any(c => c.NumberPlate == cars.NumberPlate))
            {
                ModelState.AddModelError("NumberPlate", "This number plate already exists.");
            }

            if (ModelState.IsValid)
            {
                db.Tbl_Cars.Add(cars);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Car added successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.Customers = db.Tbl_Customers.ToList(); // reload dropdown on error
            return View(cars);
        }

        // ✅ Edit existing car (GET)
        public IActionResult EditCar(int id)
        {
            var data = db.Tbl_Cars.Find(id);
            if (data == null)
            {
                TempData["ErrorMessage"] = "Car not found.";
                return RedirectToAction("Index");
            }

            ViewBag.Customers = db.Tbl_Customers.ToList();
            return View(data);
        }

        // ✅ Edit existing car (POST)
        [HttpPost]
        public IActionResult EditCar(Cars cars)
        {
            if (db.Tbl_Cars.Any(c => c.NumberPlate == cars.NumberPlate && c.CarID != cars.CarID))
            {
                ModelState.AddModelError("NumberPlate", "This number plate already exists.");
            }

            if (ModelState.IsValid)
            {
                db.Tbl_Cars.Update(cars);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Car updated successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.Customers = db.Tbl_Customers.ToList();
            return View(cars);
        }

        // ✅ Delete a car safely
        public IActionResult DeleteCar(int id)
        {
            var data = db.Tbl_Cars
                         .Include(c => c.Customer)
                         .Include(c => c.Receipts)
                         .FirstOrDefault(c => c.CarID == id);

            if (data == null)
            {
                TempData["ErrorMessage"] = "Car not found.";
                return RedirectToAction("Index");
            }

            try
            {
                // If this car has related receipts, don't delete
                if (data.Receipts != null && data.Receipts.Any())
                {
                    TempData["ErrorMessage"] = "This car cannot be deleted because it has linked receipts.";
                    return RedirectToAction("Index");
                }

                db.Tbl_Cars.Remove(data);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Car deleted successfully!";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "This car cannot be deleted because it has linked records.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
