using CarMaintenance.Data;
using CarMaintenance.Models;
using CarMaintenance.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CarMaintenance.Controllers
{
    public class ReceiptsController : Controller
    {
        private readonly AppDbContext db;

        public ReceiptsController(AppDbContext _db)
        {
            db = _db;
        }

        // Show all receipts
        public IActionResult Index()
        {
            var data = db.Tbl_Receipts
                         .Include(r => r.Customers)
                         .Include(r => r.Cars)
                         .ToList();
            return View(data);
        }

        // GET: AddReceipt
        public IActionResult AddReceipt()
        {
            var vm = new ReceiptViewModel
            {
                Date = DateTime.Now
            };
            ViewBag.Services = db.Tbl_Services.ToList();
            return View(vm);
        }

        // POST: AddReceipt
        [HttpPost]
        public IActionResult AddReceipt(ReceiptViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Services = db.Tbl_Services.ToList();
                return View(vm);
            }

            // Map NumberPlate to CarID (or create new Car)
            int carId = 0;
            if (!string.IsNullOrEmpty(vm.NumberPlate))
            {
                var car = db.Tbl_Cars.FirstOrDefault(c => c.NumberPlate == vm.NumberPlate);
                if (car != null)
                    carId = car.CarID;
                else
                {
                    var newCar = new Cars { NumberPlate = vm.NumberPlate };
                    db.Tbl_Cars.Add(newCar);
                    db.SaveChanges();
                    carId = newCar.CarID;
                }
            }

            var receipt = new Receipts
            {
                CustomerID = vm.CustomerID,
                CarID = carId,
                Date = vm.Date,
                TotalAmount = vm.TotalAmount
            };

            db.Tbl_Receipts.Add(receipt);
            db.SaveChanges();

            // Add services
            if (vm.ServicesSelected != null && vm.ServicesSelected.Any())
            {
                foreach (var item in vm.ServicesSelected)
                {
                    db.Tbl_ReceiptDetails.Add(new ReceiptsDetails
                    {
                        ReceiptID = receipt.ReceiptID,
                        ServiceID = item.ServiceID
                    });
                }
                db.SaveChanges();
            }

            // Save data for printing
            TempData["PrintReceipt"] = new
            {
                ReceiptID = receipt.ReceiptID,
                Date = receipt.Date.ToString("yyyy-MM-dd HH:mm"),
                CustomerName = db.Tbl_Customers.FirstOrDefault(c => c.CustomerID == vm.CustomerID)?.Name ?? "N/A",
                CarNumber = vm.NumberPlate,
                Services = vm.ServicesSelected.Select(s => new { s.ServiceName, s.Description, s.Price }).ToList(),
                TotalAmount = vm.TotalAmount,
                
            };

            ViewBag.Services = db.Tbl_Services.ToList();
            return View(vm);
        }

        // JSON: Search customers for autocomplete
        [HttpGet]
        public JsonResult SearchCustomers(string term)
        {
            var customers = db.Tbl_Customers
                              .Where(c => c.Name.Contains(term))
                              .Select(c => new
                              {
                                  customerID = c.CustomerID,
                                  name = c.Name
                              })
                              .ToList();
            return Json(customers);
        }

        // JSON: Get car info by customer ID
        [HttpGet]
        public JsonResult GetCarByCustomer(int customerId)
        {
            var customer = db.Tbl_Customers.Include(c => c.Cars).FirstOrDefault(c => c.CustomerID == customerId);
            return Json(new
            {
                carId = customer?.CarID ?? 0,
                NumberPlate = customer?.Cars?.NumberPlate ?? "N/A"
            });
        }
    }
}
