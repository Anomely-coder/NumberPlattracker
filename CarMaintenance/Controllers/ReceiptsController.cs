using CarMaintenance.Data;
using CarMaintenance.Models;
using CarMaintenance.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
            var data = db.Tbl_Receipts.Include(x => x.Customers).Include(y => y.Cars).ToList();

            return View(data);
        }

        public IActionResult AddReceipt()
        {
            var vm = new ReceiptViewModel
            {
                CustomersList = new SelectList(db.Tbl_Customers.Where(x => x.CarID != 0), "CustomerID", "Name"),
                ServicesList = new SelectList(db.Tbl_Services, "ServiceID", "ServiceName"),
                Date = DateTime.Now
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult AddReceipt(ReceiptViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.CustomersList = new SelectList(db.Tbl_Customers.Where(x => x.CarID != 0), "CustomerID", "Name");
                vm.ServicesList = new SelectList(db.Tbl_Services, "ServiceID", "ServiceName");
                return View(vm);
            }

            var receipt = new Receipts
            {
                CustomerID = vm.CustomerID,
                CarID = vm.CarID,
                Date = vm.Date,
                TotalAmount = vm.TotalAmount
            };

            db.Tbl_Receipts.Add(receipt);
            db.SaveChanges();

            foreach (var item in vm.ServicesSelected)
            {
                db.Tbl_ReceiptDetails.Add(new ReceiptsDetails
                {
                    ReceiptID = receipt.ReceiptID,
                    ServiceID = item.ServiceID
                });
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetCarByCustomer(int customerId)
        {
            var customer = db.Tbl_Customers.Include(x => x.Cars).FirstOrDefault(x => x.CustomerID == customerId);
            return Json(new
            {
                carId = customer?.CarID ?? 0,
                NumberPlate = customer?.Cars?.NumberPlate ?? "N/A"
            });
        }

        [HttpGet]
        public JsonResult GetServiceDetails(int serviceId)
        {
            var service = db.Tbl_Services.FirstOrDefault(x => x.ServiceID == serviceId);
            return Json(service);
        }
    }
}
