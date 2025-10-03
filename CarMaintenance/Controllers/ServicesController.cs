using CarMaintenance.Data;
using CarMaintenance.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarMaintenance.Controllers
{
    public class ServicesController : Controller
    {
        private readonly AppDbContext db;

        public ServicesController(AppDbContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            var data = db.Tbl_Services;
            return View(data);
        }

        public IActionResult AddService()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddService(Services services)
        {
            if (ModelState.IsValid)
            {
                db.Tbl_Services.Add(services);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Service added successfully.";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Failed to add service. Please check the input.";
            return View(services);
        }

        public IActionResult EditService(int Id)
        {
            var data = db.Tbl_Services.Find(Id);
            if (data == null)
            {
                TempData["ErrorMessage"] = "Service not found.";
                return RedirectToAction("Index");
            }
            return View(data);
        }

        [HttpPost]
        public IActionResult EditService(Services services)
        {
            if (ModelState.IsValid)
            {
                db.Tbl_Services.Update(services);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Service updated successfully.";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Failed to update service.";
            return View(services);
        }

        public IActionResult DeleteService(int Id)
        {
            var data = db.Tbl_Services.Find(Id);

            if (data == null)
            {
                TempData["ErrorMessage"] = "Service not found.";
                return RedirectToAction("Index");
            }

            try
            {
                db.Tbl_Services.Remove(data);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Service deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "This service cannot be deleted because it is already used in receipts.";
            }

            return RedirectToAction("Index");
        }
    }
}
