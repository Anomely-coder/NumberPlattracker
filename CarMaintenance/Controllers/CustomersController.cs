using CarMaintenance.Data;
using CarMaintenance.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CarMaintenance.Controllers
{
    public class CustomersController : Controller
    {
        private AppDbContext db;

        public CustomersController(AppDbContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            var data = db.Tbl_Customers.Include(x => x.Cars).ToList();
            return View(data);
        }

        public IActionResult AddCustomer()
        {
            // ✅ Only unregistered cars
            var unregisteredCars = db.Tbl_Cars
                .Where(c => c.CarStatus == 0)
                .ToList();

            ViewBag.Cars = new SelectList(unregisteredCars, "CarID", "NumberPlate");
            return View();
        }

        [HttpPost]
        public IActionResult AddCustomer(Customers customers)
        {
            if (db.Tbl_Customers.Any(c => c.Email == customers.Email))
            {
                ModelState.AddModelError("Email", "This Email already exists. Please enter a unique Email.");
            }

            if (db.Tbl_Customers.Any(c => c.EmiratesID == customers.EmiratesID))
            {
                ModelState.AddModelError("EmiratesID", "This Emirates ID already exists. Please enter a unique Emirates ID.");
            }

            if (ModelState.IsValid)
            {
                db.Tbl_Customers.Add(customers);

                // ✅ Update car status
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

            // ✅ Re-bind only unregistered cars if form fails
            var unregisteredCars = db.Tbl_Cars
                .Where(c => c.CarStatus == 0)
                .ToList();

            ViewBag.Cars = new SelectList(unregisteredCars, "CarID", "NumberPlate");
            return View(customers);
        }

        public IActionResult EditCustomer(int Id)
        {
            // ✅ All cars allowed in Edit
            ViewBag.Cars = new SelectList(db.Tbl_Cars.ToList(), "CarID", "NumberPlate");
            var data = db.Tbl_Customers.Find(Id);
            return View(data);
        }

        [HttpPost]
        public IActionResult EditCustomer(Customers customers)
        {
            if (db.Tbl_Customers.Any(c => c.EmiratesID == customers.EmiratesID && c.CustomerID != customers.CustomerID))
            {
                ModelState.AddModelError("EmiratesID", "This Emirates ID already exists. Please enter a unique Emirates ID.");
            }

            if (ModelState.IsValid)
            {
                var oldCustomer = db.Tbl_Customers.AsNoTracking().FirstOrDefault(c => c.CustomerID == customers.CustomerID);
                db.Tbl_Customers.Update(customers);

                // ✅ If car changed, update statuses
                if (oldCustomer?.CarID != customers.CarID)
                {
                    // Unregister old car
                    if (oldCustomer?.CarID != null)
                    {
                        var oldCar = db.Tbl_Cars.Find(oldCustomer.CarID.Value);
                        if (oldCar != null)
                        {
                            bool stillAssigned = db.Tbl_Customers.Any(c => c.CarID == oldCar.CarID && c.CustomerID != customers.CustomerID);
                            if (!stillAssigned)
                            {
                                oldCar.CarStatus = 0; // Unregistered
                                db.Tbl_Cars.Update(oldCar);
                            }
                        }
                    }

                    // Register new car
                    if (customers.CarID != null)
                    {
                        var newCar = db.Tbl_Cars.Find(customers.CarID.Value);
                        if (newCar != null)
                        {
                            newCar.CarStatus = 1; // Registered
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

        public IActionResult DeleteCustomer(int Id)
        {
            var data = db.Tbl_Customers.Find(Id);
            if (data != null)
            {
                // ✅ Unregister car if no one else owns it
                if (data.CarID.HasValue)
                {
                    var car = db.Tbl_Cars.Find(data.CarID.Value);
                    if (car != null)
                    {
                        bool stillAssigned = db.Tbl_Customers.Any(c => c.CarID == car.CarID && c.CustomerID != data.CustomerID);
                        if (!stillAssigned)
                        {
                            car.CarStatus = 0; // Unregistered
                            db.Tbl_Cars.Update(car);
                        }
                    }
                }

                db.Tbl_Customers.Remove(data);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}