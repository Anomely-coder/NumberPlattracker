using CarMaintenance.Data;
using CarMaintenance.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CarMaintenance.Controllers
{
    public class CustomersController : Controller
    {
        private readonly AppDbContext db;

        public CustomersController(AppDbContext _db)
        {
            db = _db;
        }

        // ---------------- LIST ----------------
        public IActionResult Index()
        {
            var data = db.Tbl_Customers
                .Include(x => x.Cars)
                .ToList();
            return View(data);
        }

        // ---------------- ADD ----------------
        public IActionResult AddCustomer()
        {
            // Show all cars that are unassigned and unregistered
            var unregisteredCars = db.Tbl_Cars
                .Where(c => c.CustomerID == null && c.CarStatus == 0)
                .ToList();

            ViewBag.Cars = new MultiSelectList(unregisteredCars, "CarID", "NumberPlate");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCustomer(Customers customer, int[] selectedCarIDs)
        {
            // Unique field validation
            if (db.Tbl_Customers.Any(c => c.Email == customer.Email))
                ModelState.AddModelError("Email", "This Email already exists.");

            if (db.Tbl_Customers.Any(c => c.EmiratesID == customer.EmiratesID))
                ModelState.AddModelError("EmiratesID", "This Emirates ID already exists.");

            if (db.Tbl_Customers.Any(c => c.MobileNumber == customer.MobileNumber))
                ModelState.AddModelError("MobileNumber", "This Mobile Number already exists.");

            if (ModelState.IsValid)
            {
                customer.CreatedAt = DateTime.Now;
                db.Tbl_Customers.Add(customer);
                db.SaveChanges(); // Generate CustomerID

                // ✅ Link selected cars to this customer
                if (selectedCarIDs != null && selectedCarIDs.Length > 0)
                {
                    var cars = db.Tbl_Cars.Where(c => selectedCarIDs.Contains(c.CarID)).ToList();
                    foreach (var car in cars)
                    {
                        car.CustomerID = customer.CustomerID;
                        car.CarStatus = 1;
                    }
                    db.Tbl_Cars.UpdateRange(cars);
                    db.SaveChanges();
                }

                TempData["SuccessMessage"] = "Customer added successfully!";
                return RedirectToAction("Index");
            }

            // Re-bind cars if validation fails
            var unregisteredCars = db.Tbl_Cars
                .Where(c => c.CustomerID == null && c.CarStatus == 0)
                .ToList();
            ViewBag.Cars = new MultiSelectList(unregisteredCars, "CarID", "NumberPlate", selectedCarIDs);

            return View(customer);
        }

        // ---------------- EDIT ----------------
        public IActionResult EditCustomer(int id)
        {
            var customer = db.Tbl_Customers
                .Include(c => c.Cars)
                .FirstOrDefault(c => c.CustomerID == id);

            if (customer == null) return NotFound();

            var availableCars = db.Tbl_Cars
                .Where(c => c.CustomerID == null || c.CustomerID == id)
                .ToList();

            var selectedIds = customer.Cars.Select(c => c.CarID).ToArray();

            ViewBag.Cars = new MultiSelectList(availableCars, "CarID", "NumberPlate", selectedIds);
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCustomer(Customers customer, int[] selectedCarIDs)
        {
            if (db.Tbl_Customers.Any(c => c.EmiratesID == customer.EmiratesID && c.CustomerID != customer.CustomerID))
                ModelState.AddModelError("EmiratesID", "This Emirates ID already exists.");

            if (ModelState.IsValid)
            {
                customer.UpdatedAt = DateTime.Now;
                db.Tbl_Customers.Update(customer);
                db.SaveChanges();

                // Unlink cars that were previously assigned but not selected
                var assignedCars = db.Tbl_Cars.Where(c => c.CustomerID == customer.CustomerID).ToList();
                foreach (var car in assignedCars)
                {
                    if (!selectedCarIDs.Contains(car.CarID))
                    {
                        car.CustomerID = null;
                        car.CarStatus = 0;
                    }
                }

                // Link new cars
                var newCars = db.Tbl_Cars.Where(c => selectedCarIDs.Contains(c.CarID)).ToList();
                foreach (var car in newCars)
                {
                    car.CustomerID = customer.CustomerID;
                    car.CarStatus = 1;
                }

                db.SaveChanges();
                TempData["SuccessMessage"] = "Customer updated successfully!";
                return RedirectToAction("Index");
            }

            var availableCars = db.Tbl_Cars
                .Where(c => c.CustomerID == null || c.CustomerID == customer.CustomerID)
                .ToList();
            ViewBag.Cars = new MultiSelectList(availableCars, "CarID", "NumberPlate", selectedCarIDs);

            return View(customer);
        }

        // ---------------- DELETE ----------------
        [HttpGet]
        public IActionResult DeleteCustomer(int id)
        {
            var customer = db.Tbl_Customers
                .Include(c => c.Cars)
                .FirstOrDefault(c => c.CustomerID == id);

            if (customer == null)
            {
                TempData["ErrorMessage"] = "Customer not found.";
                return RedirectToAction("Index");
            }

            try
            {
                // Unlink cars before deletion
                foreach (var car in customer.Cars)
                {
                    car.CustomerID = null;
                    car.CarStatus = 0;
                }

                db.Tbl_Cars.UpdateRange(customer.Cars);
                db.Tbl_Customers.Remove(customer);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Customer deleted successfully.";
            }
            catch
            {
                TempData["ErrorMessage"] = "Cannot delete this customer because there are related receipts.";
            }

            return RedirectToAction("Index");
        }
    }
}
