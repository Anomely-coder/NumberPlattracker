using CarMaintenance.Data;
using CarMaintenance.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarMaintenance.Controllers
{
    public class TransferCarsController : Controller
    {
        private AppDbContext db;

        public TransferCarsController(AppDbContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            var data = db.Tbl_TransferCars.Include(x => x.FromCustomers).Include(y => y.ToCustomers).Include(z => z.Cars).ToList();

            return View(data);
        }

        public IActionResult Transfer()
        {
            ViewBag.Customers = new SelectList(db.Tbl_Customers.Where(x => x.CarID != 0).ToList(), "CustomerID", "Name");

            var model = new TransferCars
            {
                TransferDate = DateTime.Now
            };

            return View(model);

        }

        [HttpGet]
        public IActionResult GetCarByCustomer(int customerId)
        {
            var car = db.Tbl_Customers
                        .Where(c => c.CustomerID == customerId)
                        .Select(c => new
                        {
                            c.CarID,
                            c.Cars.NumberPlate // Adjust this property name if different
                        }).FirstOrDefault();

            return Json(car);
        }

        [HttpPost]
        public IActionResult Transfer(TransferCars model)
        {
            if (ModelState.IsValid)
            {
                var fromCustomer = db.Tbl_Customers.Find(model.FromCustomerID);
                var toCustomer = db.Tbl_Customers.Find(model.ToCustomerID);

                if (fromCustomer != null && toCustomer != null)
                {
                    // 1. Update old customer’s CarID to 0
                    fromCustomer.CarID = null;


                    // 2. Assign the car to the new customer
                    toCustomer.CarID = model.CarID;

                    // 3. Save transfer record
                    db.Tbl_TransferCars.Add(model);

                    db.SaveChanges();

                    TempData["success"] = "Car transferred successfully.";
                    return RedirectToAction("Index");
                }
            }

            ViewBag.Customers = new SelectList(db.Tbl_Customers.ToList(), "CustomerID", "Name");
            return View(model);
        }



    }
}
