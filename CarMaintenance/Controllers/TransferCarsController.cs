using CarMaintenance.Data;
using CarMaintenance.Models;
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
        public IActionResult Transfer()
        {
            var model = new TransferCars
            {
                TransferDate = DateTime.Now
            };
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

        // ✅ Fetch Number Plates for a selected customer
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

        // ✅ Save transfer
        [HttpPost]
        public IActionResult Transfer(TransferCars model)
        {
            if (ModelState.IsValid)
            {
                var car = db.Tbl_Cars.FirstOrDefault(c => c.CarID == model.CarID);

                if (car == null)
                {
                    TempData["error"] = "Car not found.";
                    return View(model);
                }

                var fromCustomer = db.Tbl_Customers
                    .Include(c => c.Cars)
                    .FirstOrDefault(c => c.CustomerID == model.FromCustomerID);

                var toCustomer = db.Tbl_Customers
                    .Include(c => c.Cars)
                    .FirstOrDefault(c => c.CustomerID == model.ToCustomerID);

                if (fromCustomer != null && toCustomer != null)
                {
                    if (car.CustomerID != fromCustomer.CustomerID)
                    {
                        TempData["error"] = "This car does not belong to the selected source customer.";
                        return View(model);
                    }

                    car.CustomerID = toCustomer.CustomerID;

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
