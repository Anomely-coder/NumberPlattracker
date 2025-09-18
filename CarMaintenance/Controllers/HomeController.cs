using CarMaintenance.Data;
using Microsoft.AspNetCore.Mvc;

namespace CarMaintenance.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext db;

        public HomeController(AppDbContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                Console.WriteLine("No user session, redirecting to Login");
                return RedirectToAction("Login", "Account");
            }

            // Set ViewData for dashboard counts
            ViewData["TotalCustomers"] = db.Tbl_Users.Count(u => u.Role == "User");
            ViewData["TotalCars"] = db.Tbl_Cars?.Count() ?? 0; // Adjust table name as needed
            ViewData["TotalServices"] = db.Tbl_Services?.Count() ?? 0; // Adjust table name as needed
            ViewData["TotalUsers"] = db.Tbl_Users.Count();

            return View();
        }
    }
}