using CarMaintenance.Data;
using CarMaintenance.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarMaintenance.Controllers
{
    public class CarsController : Controller
    {
        private AppDbContext db;

        public CarsController(AppDbContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            var data = db.Tbl_Cars;
            return View(data);
        }

        public IActionResult AddCar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCar(Cars cars)
        {
            if (ModelState.IsValid)
            {
                db.Tbl_Cars.Add(cars);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult EditCar(int Id)
        {
            var data = db.Tbl_Cars.Find(Id);
            return View(data);
        }

        [HttpPost]
        public IActionResult EditCar(Cars cars)
        {
            if (ModelState.IsValid)
            {
                db.Tbl_Cars.Update(cars);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult DeleteCar(int Id)
        {
            var data = db.Tbl_Cars.Find(Id);

            if (data != null)
            {
                db.Tbl_Cars.Remove(data);
                db.SaveChanges();

            }
            return RedirectToAction("Index");
        }
    }
}
