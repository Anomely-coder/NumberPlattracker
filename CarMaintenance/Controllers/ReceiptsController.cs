using CarMaintenance.Data;
using CarMaintenance.Models;
using CarMaintenance.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public IActionResult Index()
        {
            var data = db.Tbl_Receipts
                         .Include(r => r.Customers)
                         .Include(r => r.Cars)
                         .ToList();
            return View(data);
        }

        // ---------------- GET: Add Receipt ----------------
        public IActionResult AddReceipt()
        {
            var vm = new ReceiptViewModel
            {
                Date = DateTime.Now
            };
            ViewBag.Services = db.Tbl_Services.ToList();
            return View(vm);
        }

        // ---------------- POST: Add Receipt ----------------
        [HttpPost]
        public IActionResult AddReceipt(ReceiptViewModel vm)
        {
            ViewBag.Services = db.Tbl_Services.ToList();

            if (!ModelState.IsValid)
                return View(vm);

            int carId = vm.CarID;

            var receipt = new Receipts
            {
                CustomerID = vm.CustomerID,
                CarID = carId,
                Date = vm.Date,
                TotalAmount = vm.TotalAmount
            };

            db.Tbl_Receipts.Add(receipt);
            db.SaveChanges();

            var servicesList = new List<object>();
            if (vm.ServicesSelected != null && vm.ServicesSelected.Any())
            {
                foreach (var item in vm.ServicesSelected)
                {
                    db.Tbl_ReceiptDetails.Add(new ReceiptsDetails
                    {
                        ReceiptID = receipt.ReceiptID,
                        ServiceID = item.ServiceID
                    });

                    servicesList.Add(new
                    {
                        serviceName = item.ServiceName,
                        description = item.Description,
                        price = item.Price
                    });
                }
                db.SaveChanges();
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var customer = db.Tbl_Customers
                    .Include(c => c.Cars)
                    .FirstOrDefault(c => c.CustomerID == vm.CustomerID);

                var selectedCar = db.Tbl_Cars.FirstOrDefault(c => c.CarID == vm.CarID);

                return Json(new
                {
                    receiptID = receipt.ReceiptID,
                    date = receipt.Date.ToString("yyyy-MM-dd HH:mm"),
                    customerName = customer?.Name ?? "N/A",
                    carNumber = selectedCar?.NumberPlate ?? "N/A",
                    services = servicesList,
                    totalAmount = vm.TotalAmount
                });
            }

            return RedirectToAction("Index");
        }

        // ---------------- AJAX: Customer Search ----------------
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

        // ---------------- AJAX: Get Cars for Selected Customer ----------------
        [HttpGet]
        public JsonResult GetCarsByCustomer(int customerId)
        {
            var cars = db.Tbl_Cars
                         .Where(c => c.CustomerID == customerId)
                         .Select(c => new
                         {
                             carId = c.CarID,
                             numberPlate = c.NumberPlate
                         })
                         .ToList();
            return Json(cars);
        }

        // ---------------- GET: Receipt Details ----------------
        public IActionResult Details(int id)
        {
            var receipt = db.Tbl_Receipts
                            .Include(r => r.Customers)
                            .Include(r => r.Cars)
                            .Include(r => r.ReceiptsDetails)
                                .ThenInclude(rd => rd.Services)
                            .FirstOrDefault(r => r.ReceiptID == id);

            if (receipt == null)
                return NotFound();

            return View(receipt);
        }
    }
}
