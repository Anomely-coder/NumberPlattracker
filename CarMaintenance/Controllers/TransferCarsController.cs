using CarMaintenance.Data;
using CarMaintenance.Models;
using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CarMaintenance.Controllers
{
    public class TransferCarsController : Controller
    {
        private readonly AppDbContext db;

        public TransferCarsController(AppDbContext _db)
        {
            db = _db;
        }

        // ✅ List all car transfers
        public IActionResult Index()
        {
            var data = db.Tbl_TransferCars
                .Include(x => x.FromCustomers)
                .Include(y => y.ToCustomers)
                .Include(z => z.Cars)
                .ToList();

            return View(data);
        }

        // ✅ Show transfer form
        public IActionResult Transfer(string mode = "transfer")
        {
            var model = new TransferCars
            {
                TransferDate = DateTime.Now
            };
            ViewBag.Mode = mode; // "transfer" or "assign"
            return View(model);
        }

        // ✅ Autocomplete API for customer search
        [HttpGet]
        public IActionResult SearchCustomers(string term)
        {
            term = term?.Trim() ?? "";

            var customers = db.Tbl_Customers
                .Include(c => c.Cars)
                .Where(c =>
                    c.Name.Contains(term) ||
                    c.EmiratesID.Contains(term) ||
                    c.MobileNumber.Contains(term))
                .Select(c => new
                {
                    customerID = c.CustomerID,
                    name = c.Name,
                    emiratesId = c.EmiratesID,
                    mobile = c.MobileNumber
                })
                .ToList();

            return Json(customers);
        }

        // ✅ Fetch Number Plates for a selected customer (used in Transfer mode)
        [HttpGet]
        public IActionResult GetNumberPlates(int customerId)
        {
            var plates = db.Tbl_Cars
                .Where(c => c.CustomerID == customerId)
                .Select(c => new
                {
                    carID = c.CarID,
                    numberPlate = c.NumberPlate
                })
                .ToList();

            return Json(plates);
        }

        // ✅ Fetch unassigned cars (used in Assign mode)
        [HttpGet]
        public IActionResult GetUnassignedCars()
        {
            // ✅ Only cars that are not assigned to any customer
            var unassignedCars = db.Tbl_Cars
                .Where(c => c.CustomerID == null)
                .Select(c => new
                {
                    carID = c.CarID,
                    numberPlate = c.NumberPlate
                })
                .ToList();

            return Json(unassignedCars);
        }

        // ✅ Save transfer or assignment
        [HttpPost]
        public IActionResult Transfer(TransferCars model, string mode = "transfer")
        {
            if (ModelState.IsValid)
            {
                var car = db.Tbl_Cars.FirstOrDefault(c => c.CarID == model.CarID);
                if (car == null)
                {
                    TempData["error"] = "Car not found.";
                    return View(model);
                }

                // === ASSIGN MODE ===
                if (mode == "assign")
                {
                    var toCustomer = db.Tbl_Customers.FirstOrDefault(c => c.CustomerID == model.ToCustomerID);
                    if (toCustomer == null)
                    {
                        TempData["error"] = "Target customer not found.";
                        return View(model);
                    }

                    if (car.CustomerID != null)
                    {
                        TempData["error"] = "This car is already assigned.";
                        return View(model);
                    }

                    car.CustomerID = toCustomer.CustomerID;
                    db.SaveChanges();

                    TempData["success"] = "Car assigned successfully.";
                    return RedirectToAction("Index");
                }

                // === TRANSFER MODE ===
                var fromCustomer = db.Tbl_Customers
                    .Include(c => c.Cars)
                    .FirstOrDefault(c => c.CustomerID == model.FromCustomerID);

                var toCust = db.Tbl_Customers
                    .Include(c => c.Cars)
                    .FirstOrDefault(c => c.CustomerID == model.ToCustomerID);

                if (fromCustomer != null && toCust != null)
                {
                    if (car.CustomerID != fromCustomer.CustomerID)
                    {
                        TempData["error"] = "This car does not belong to the selected source customer.";
                        return View(model);
                    }

                    car.CustomerID = toCust.CustomerID;
                    db.Tbl_TransferCars.Add(model);
                    db.SaveChanges();

                    TempData["success"] = "Car transferred successfully.";
                    return RedirectToAction("Index");
                }
            }

            TempData["error"] = "Something went wrong. Please try again.";
            return View(model);
        }
    }
}
