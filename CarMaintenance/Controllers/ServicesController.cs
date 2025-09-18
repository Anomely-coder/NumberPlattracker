using CarMaintenance.Data;
using CarMaintenance.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarMaintenance.Controllers
{
    public class ServicesController : Controller
    {
        private AppDbContext db;

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

                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult EditService(int Id)
        {
            var data = db.Tbl_Services.Find(Id);
            return View(data);
        }

        [HttpPost]
        public IActionResult EditService(Services services)
        {
            if (ModelState.IsValid)
            {
                db.Tbl_Services.Update(services);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult DeleteService(int Id)
        {
            var data = db.Tbl_Services.Find(Id);

            if (data != null)
            {
                db.Tbl_Services.Remove(data);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
