using CarMaintenance.Data;
using CarMaintenance.Models;
using CarMaintenance.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using NLog; // Ensure this is included for NLog.ILogger

namespace CarMaintenance.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext db;
        private readonly NLog.ILogger logger = LogManager.GetCurrentClassLogger(); // Explicitly use NLog.ILogger

        public AccountController(AppDbContext _db)
        {
            db = _db;
        }

        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel login)
        {
            logger.Info($"Login attempt for Email: '{login.Email}'");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                logger.Error("Login failed due to validation errors: " + string.Join(", ", errors));
                ViewBag.Message = "Validation failed: " + string.Join(", ", errors);
                return View(login);
            }

            logger.Info($"Looking up user with Email: '{login.Email.Trim().ToLower()}'");
            var user = db.Tbl_Users
                .FirstOrDefault(x => x.Email.Trim().ToLower() == login.Email.Trim().ToLower()
                                  && x.Password == login.Password);

            if (user == null)
            {
                logger.Warn($"Login failed: No user found for Email: '{login.Email}' or incorrect password");
                ViewBag.Message = "Invalid Email or Password";
                return View(login);
            }

            logger.Info($"User found: Email: '{user.Email}', UserID: {user.UserID}, Role: {user.Role}");

            bool isFirstLogin = !user.LastLogin.HasValue;
            bool isPasswordExpired = user.PasswordLastChanged.HasValue &&
                (DateTime.Now - user.PasswordLastChanged.Value).TotalDays > 61;

            user.LastLogin = DateTime.Now;
            try
            {
                db.SaveChanges();
                logger.Info($"Updated LastLogin for UserID: {user.UserID}");
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Failed to update LastLogin for UserID: {user.UserID}");
                ViewBag.Message = "Error saving login time: " + ex.Message;
                return View(login);
            }

            // Create claims for the user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            try
            {
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);
                HttpContext.Session.SetInt32("UserId", user.UserID); // Set session for ChangePassword compatibility
                logger.Info($"Authentication successful for UserID: {user.UserID}, Session set");
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Authentication failed for UserID: {user.UserID}");
                ViewBag.Message = "Authentication error: " + ex.Message;
                return View(login);
            }

            if (isFirstLogin || isPasswordExpired)
            {
                logger.Info($"Redirecting to ChangePassword for UserID: {user.UserID}, FirstLogin: {isFirstLogin}, Expired: {isPasswordExpired}");
                return RedirectToAction("ChangePassword");
            }

            logger.Info($"Redirecting to Home/Index for UserID: {user.UserID}");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                Console.WriteLine("ModelState Errors: " + string.Join(", ", errors));
                ViewBag.Message = "Validation failed: " + string.Join(", ", errors);
                return View(model);
            }

            var user = db.Tbl_Users
                .FirstOrDefault(x => x.Email.Trim().ToLower() == model.Email.Trim().ToLower());

            if (user != null)
            {
                string newPassword = PasswordHelper.GeneratePassword();
                while (PasswordHelper.IsPasswordInHistory(newPassword, user.UserID, db))
                {
                    newPassword = PasswordHelper.GeneratePassword();
                }
                user.Password = newPassword;
                user.PasswordLastChanged = DateTime.Now;
                db.Tbl_PasswordHistory.Add(new PasswordHistory
                {
                    UserID = user.UserID,
                    Password = newPassword,
                    CreatedAt = DateTime.Now
                });
                db.SaveChanges();
                Console.WriteLine($"New Password for {user.Email}: {newPassword}");
                TempData["Message"] = $"Password reset for {user.Email}. New password: {newPassword}";
                return RedirectToAction("Login");
            }

            Console.WriteLine($"Forgot Password failed: No user found for {model.Email}");
            ViewBag.Message = "No user found with this email address.";
            return View(model);
        }

        public IActionResult ChangePassword()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                Console.WriteLine("No user session, redirecting to Login");
                return RedirectToAction("Login");
            }
            return View(new ChangePasswordModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordModel model)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                Console.WriteLine("No user session, redirecting to Login");
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                Console.WriteLine("ModelState Errors: " + string.Join(", ", errors));
                ViewBag.Message = "Validation failed: " + string.Join(", ", errors);
                return View(model);
            }

            int userId = HttpContext.Session.GetInt32("UserId").Value;
            var user = db.Tbl_Users.Find(userId);
            if (user == null)
            {
                Console.WriteLine($"Change Password failed: UserID {userId} not found");
                return NotFound();
            }

            if (user.Password != model.OldPassword)
            {
                ModelState.AddModelError("OldPassword", "Current password is incorrect.");
                Console.WriteLine("Change Password failed: Incorrect old password");
                ViewBag.Message = "Current password is incorrect.";
                return View(model);
            }

            if (!PasswordHelper.ValidatePassword(model.NewPassword))
            {
                ModelState.AddModelError("NewPassword", "New password does not meet policy requirements.");
                Console.WriteLine("Change Password failed: Invalid new password");
                ViewBag.Message = "New password does not meet policy requirements.";
                return View(model);
            }

            if (PasswordHelper.IsPasswordInHistory(model.NewPassword, userId, db))
            {
                ModelState.AddModelError("NewPassword", "New password cannot be one of the last two used passwords.");
                Console.WriteLine("Change Password failed: New password matches recent password");
                ViewBag.Message = "New password cannot be one of the last two used passwords.";
                return View(model);
            }

            user.Password = model.NewPassword;
            user.PasswordLastChanged = DateTime.Now;
            db.Tbl_PasswordHistory.Add(new PasswordHistory
            {
                UserID = user.UserID,
                Password = model.NewPassword,
                CreatedAt = DateTime.Now
            });

            // Keep only the last two password history entries
            var oldPasswords = db.Tbl_PasswordHistory
                .Where(ph => ph.UserID == userId)
                .OrderByDescending(ph => ph.CreatedAt)
                .Skip(2)
                .ToList();
            db.Tbl_PasswordHistory.RemoveRange(oldPasswords);

            db.SaveChanges();
            Console.WriteLine($"Password changed for {user.Email}");
            TempData["Message"] = "Password changed successfully.";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            return RedirectToAction("Login", "Account");
        }
    }
}