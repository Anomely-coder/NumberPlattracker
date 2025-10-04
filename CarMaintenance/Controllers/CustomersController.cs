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
            var unregisteredCars = db.Tbl_Cars
                .Where(c => c.CarStatus == 0)
                .ToList();

            ViewBag.Cars = new SelectList(unregisteredCars, "CarID", "NumberPlate");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCustomer(Customers customers)
        {
            // Unique checks
            if (db.Tbl_Customers.Any(c => c.Email == customers.Email))
            {
                ModelState.AddModelError("Email", "This Email already exists. Please enter a unique Email.");
            }
            if (db.Tbl_Customers.Any(c => c.EmiratesID == customers.EmiratesID))
            {
                ModelState.AddModelError("EmiratesID", "This Emirates ID already exists. Please enter a unique Emirates ID.");
            }
            if (db.Tbl_Customers.Any(c => c.MobileNumber == customers.MobileNumber))
            {
                ModelState.AddModelError("MobileNumber", "This Mobile Number already exists. Please enter a unique Mobile Number.");
            }

            if (ModelState.IsValid)
            {
                // set CreatedAt
                customers.CreatedAt = DateTime.Now;
                customers.UpdatedAt = null;

                db.Tbl_Customers.Add(customers);

                // Update car status if linked
                if (customers.CarID.HasValue)
                {
                    var car = db.Tbl_Cars.Find(customers.CarID.Value);
                    if (car != null)
                    {
                        car.CarStatus = 1; // Registered
                        db.Tbl_Cars.Update(car);
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Re-bind dropdown
            var unregisteredCars = db.Tbl_Cars
                .Where(c => c.CarStatus == 0)
                .ToList();
            ViewBag.Cars = new SelectList(unregisteredCars, "CarID", "NumberPlate");
            return View(customers);
        }

        // ---------------- EDIT ----------------
        public IActionResult EditCustomer(int id)
        {
            var data = db.Tbl_Customers.Find(id);
            if (data == null) return NotFound();

            ViewBag.Cars = new SelectList(db.Tbl_Cars.ToList(), "CarID", "NumberPlate");
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCustomer(Customers customers)
        {
            if (db.Tbl_Customers.Any(c => c.EmiratesID == customers.EmiratesID && c.CustomerID != customers.CustomerID))
            {
                ModelState.AddModelError("EmiratesID", "This Emirates ID already exists. Please enter a unique Emirates ID.");
            }

            if (ModelState.IsValid)
            {
                var oldCustomer = db.Tbl_Customers.AsNoTracking()
                    .FirstOrDefault(c => c.CustomerID == customers.CustomerID);

                // set UpdatedAt on update
                customers.UpdatedAt = DateTime.Now;

                db.Tbl_Customers.Update(customers);

                // If car changed, update old/new status
                if (oldCustomer?.CarID != customers.CarID)
                {
                    if (oldCustomer?.CarID != null)
                    {
                        var oldCar = db.Tbl_Cars.Find(oldCustomer.CarID.Value);
                        if (oldCar != null)
                        {
                            bool stillAssigned = db.Tbl_Customers
                                .Any(c => c.CarID == oldCar.CarID && c.CustomerID != customers.CustomerID);
                            if (!stillAssigned)
                            {
                                oldCar.CarStatus = 0;
                                db.Tbl_Cars.Update(oldCar);
                            }
                        }
                    }

                    if (customers.CarID != null)
                    {
                        var newCar = db.Tbl_Cars.Find(customers.CarID.Value);
                        if (newCar != null)
                        {
                            newCar.CarStatus = 1;
                            db.Tbl_Cars.Update(newCar);
                        }
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Cars = new SelectList(db.Tbl_Cars.ToList(), "CarID", "NumberPlate");
            return View(customers);
        }

        // ---------------- DELETE ----------------
        [HttpGet]
        public IActionResult DeleteCustomer(int id)
        {
            var customer = db.Tbl_Customers.Find(id);
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Customer not found.";
                return RedirectToAction("Index");
            }

            try
            {
                db.Tbl_Customers.Remove(customer);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Customer deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                // This is where the foreign key conflict happens
                TempData["ErrorMessage"] = "Cannot delete this customer because there are related receipts.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the customer.";
            }

            return RedirectToAction("Index");
        }
    }
}
