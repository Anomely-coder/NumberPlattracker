using CarMaintenance.Data;
using CarMaintenance.Models;
using Microsoft.AspNetCore.Mvc;
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCustomer(Customers customer)
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
                db.SaveChanges();

                TempData["SuccessMessage"] = "Customer added successfully!";
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // ---------------- EDIT ----------------
        public IActionResult EditCustomer(int id)
        {
            var customer = db.Tbl_Customers.FirstOrDefault(c => c.CustomerID == id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCustomer(Customers customer)
        {
            if (db.Tbl_Customers.Any(c => c.EmiratesID == customer.EmiratesID && c.CustomerID != customer.CustomerID))
                ModelState.AddModelError("EmiratesID", "This Emirates ID already exists.");

            if (ModelState.IsValid)
            {
                customer.UpdatedAt = DateTime.Now;
                db.Tbl_Customers.Update(customer);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Customer updated successfully!";
                return RedirectToAction("Index");
            }

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
