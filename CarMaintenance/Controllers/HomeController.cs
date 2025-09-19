using CarMaintenance.Data;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;

namespace CarMaintenance.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext db;
        private readonly ILogger<HomeController> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public HomeController(
            AppDbContext _db,
            ILogger<HomeController> logger,
            IStringLocalizer<SharedResource> localizer)
        {
            db = _db;
            _logger = logger;
            _localizer = localizer;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                _logger.LogInformation("No user session, redirecting to Login");
                return RedirectToAction("Login", "Account");
            }

            ViewData["Title"] = _localizer["AppTitle"];
            ViewData["TotalCustomers"] = db.Tbl_Users.Count(u => u.Role == "User");
            ViewData["TotalCars"] = db.Tbl_Cars?.Count() ?? 0;
            ViewData["TotalServices"] = db.Tbl_Services?.Count() ?? 0;
            ViewData["TotalUsers"] = db.Tbl_Users.Count();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetLanguage(string culture, string returnUrl = null)
        {
            _logger.LogDebug("SetLanguage action triggered");
            if (string.IsNullOrEmpty(culture))
            {
                _logger.LogWarning("Culture parameter is missing");
                return BadRequest("Culture parameter is required.");
            }

            _logger.LogInformation("Setting culture to: {Culture}", culture);
            _logger.LogInformation("Return URL: {ReturnUrl}", returnUrl ?? "Defaulting to Home/Index");

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            _logger.LogInformation("Cookie set for culture: {Culture}", culture);

            // Prevent open redirect attacks: only redirect to local URLs
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                returnUrl = Url.Action("Index", "Home");
            }

            return LocalRedirect(returnUrl);
        }
    }
}
