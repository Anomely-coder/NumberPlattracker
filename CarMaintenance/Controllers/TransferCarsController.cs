using CarMaintenance.Data;
using CarMaintenance.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarMaintenance.Controllers
{
    public class TransferCarsController : Controller
    {
        private readonly AppDbContext db;

        public TransferCarsController(AppDbContext _db)
        {
            db = _db;
        }

        // List of transfers
        public IActionResult Index()
        {
            var data = db.Tbl_TransferCars
                .Include(x => x.FromCustomers)
                .Include(y => y.ToCustomers)
                .Include(z => z.Cars)
                .ToList();

            return View(data);
        }

        // Show transfer form
        public IActionResult Transfer()
        {
            var model = new TransferCars
            {
                TransferDate = DateTime.Now
            };
            return View(model);
        }

        // Autocomplete API
        [HttpGet]
        public IActionResult SearchCustomers(string term)
        {
            var customers = db.Tbl_Customers
                .Where(c =>
                    c.Name.Contains(term) ||
                    c.EmiratesID.Contains(term) ||
                    c.MobileNumber.Contains(term))
                .Select(c => new
                {
                    customerID = c.CustomerID,
                    name = c.Name,
                    emiratesId = c.EmiratesID,
                    mobile = c.MobileNumber,
                    carID = c.CarID,
                    numberPlate = c.Cars != null ? c.Cars.NumberPlate : ""
                })
                .ToList();

            return Json(customers);
        }

        // Save transfer
        [HttpPost]
        public IActionResult Transfer(TransferCars model)
        {
            if (ModelState.IsValid)
            {
                var fromCustomer = db.Tbl_Customers.Find(model.FromCustomerID);
                var toCustomer = db.Tbl_Customers.Find(model.ToCustomerID);

                if (fromCustomer != null && toCustomer != null)
                {
                    // 1. Remove car from old customer
                    fromCustomer.CarID = null;

                    // 2. Assign car to new customer
                    toCustomer.CarID = model.CarID;

                    // 3. Save transfer record
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
