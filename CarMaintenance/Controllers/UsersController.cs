using CarMaintenance.Data;
using CarMaintenance.Models;
using CarMaintenance.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization; // Added for role-based authorization

namespace CarMaintenance.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContext db;

        public UsersController(AppDbContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            var data = db.Tbl_Users.ToList();
            Console.WriteLine($"Fetched {data.Count} users from Tbl_Users");
            return View(data);
        }

        public IActionResult AddUser()
        {
            return View(new Users());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUser(Users model)
        {
            if (!ModelState.IsValid)
            {
                // Remove password required error if password is empty
                if (string.IsNullOrWhiteSpace(model.Password) && ModelState.ContainsKey("Password"))
                {
                    ModelState["Password"].Errors.Clear();
                    ModelState["Password"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage).ToList();
                    Console.WriteLine("ModelState Errors: " + string.Join(", ", errors));
                    ViewBag.Message = "Validation failed: " + string.Join(", ", errors);
                    return View(model);
                }
            }

            if (!string.IsNullOrWhiteSpace(model.Password) && !PasswordHelper.ValidatePassword(model.Password))
            {
                ModelState.AddModelError("Password", "Password does not meet policy requirements.");
                Console.WriteLine("Password validation failed");
                ViewBag.Message = "Password does not meet policy requirements.";
                return View(model);
            }

            try
            {
                var user = new Users
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    MobileNumber = model.MobileNumber,
                    Email = model.Email,
                    Password = string.IsNullOrWhiteSpace(model.Password) ? PasswordHelper.GeneratePassword() : model.Password,
                    Role = model.Role,
                    CreatedAt = DateTime.Now,
                    PasswordLastChanged = DateTime.Now,
                    UserStatus = true
                };

                Console.WriteLine($"Generated Password for {user.Email}: {user.Password}");
                db.Tbl_Users.Add(user);
                db.SaveChanges();

                // Add to password history
                db.Tbl_PasswordHistory.Add(new PasswordHistory
                {
                    UserID = user.UserID,
                    Password = user.Password,
                    CreatedAt = DateTime.Now
                });
                db.SaveChanges();

                Console.WriteLine($"Saved user {user.Email} to database.");
                TempData["Message"] = $"User {user.Email} added successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving user: {ex.Message}");
                ViewBag.Message = "Error adding user: " + ex.Message;
                return View(model);
            }
        }

        [Authorize(Roles = "Admin")] // Restrict to Admin role
        public IActionResult EditUser(int Id)
        {
            var data = db.Tbl_Users.Find(Id);
            if (data == null) return NotFound();
            var model = new Users
            {
                UserID = data.UserID,
                FirstName = data.FirstName,
                LastName = data.LastName,
                MobileNumber = data.MobileNumber,
                Email = data.Email,
                Role = data.Role,
                UserStatus = data.UserStatus
                // Password is not populated to avoid exposing it
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] // Restrict to Admin role
        public IActionResult EditUser(Users model)
        {
            var existingUser = db.Tbl_Users.Find(model.UserID);
            if (existingUser == null) return NotFound();

            if (!ModelState.IsValid)
            {
                // Remove password required error if password is empty
                if (string.IsNullOrWhiteSpace(model.Password) && ModelState.ContainsKey("Password"))
                {
                    ModelState["Password"].Errors.Clear();
                    ModelState["Password"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage).ToList();
                    Console.WriteLine("ModelState Errors: " + string.Join(", ", errors));
                    ViewBag.Message = "Validation failed: " + string.Join(", ", errors);
                    return View(model);
                }
            }

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (!PasswordHelper.ValidatePassword(model.Password))
                {
                    ModelState.AddModelError("Password", "Password does not meet policy requirements.");
                    Console.WriteLine("Password validation failed");
                    ViewBag.Message = "Password does not meet policy requirements.";
                    return View(model);
                }
                if (PasswordHelper.IsPasswordInHistory(model.Password, existingUser.UserID, db))
                {
                    ModelState.AddModelError("Password", "Password cannot be one of the last two used passwords.");
                    Console.WriteLine("Password validation failed: Matches recent password");
                    ViewBag.Message = "Password cannot be one of the last two used passwords.";
                    return View(model);
                }
            }

            existingUser.FirstName = model.FirstName;
            existingUser.LastName = model.LastName;
            existingUser.MobileNumber = model.MobileNumber;
            existingUser.Email = model.Email;
            existingUser.Role = model.Role;
            existingUser.UserStatus = model.UserStatus;

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                existingUser.Password = model.Password;
                existingUser.PasswordLastChanged = DateTime.Now;
                db.Tbl_PasswordHistory.Add(new PasswordHistory
                {
                    UserID = existingUser.UserID,
                    Password = model.Password,
                    CreatedAt = DateTime.Now
                });
            }

            try
            {
                db.SaveChanges();
                Console.WriteLine($"Updated user {existingUser.Email}");
                TempData["Message"] = $"User {existingUser.Email} updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                ViewBag.Message = "Error updating user: " + ex.Message;
                return View(model);
            }
        }

        [Authorize(Roles = "Admin")] // Restrict to Admin role
        public IActionResult DeleteUser(int Id)
        {
            var data = db.Tbl_Users.Find(Id);
            if (data != null)
            {
                db.Tbl_PasswordHistory.RemoveRange(db.Tbl_PasswordHistory.Where(ph => ph.UserID == Id));
                db.Tbl_Users.Remove(data);
                db.SaveChanges();
                Console.WriteLine($"Deleted user ID {Id}");
                TempData["Message"] = $"User deleted successfully.";
            }
            else
            {
                TempData["Message"] = "User not found.";
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")] // Restrict to Admin role
        public IActionResult ResetPassword(int Id)
        {
            var user = db.Tbl_Users.Find(Id);
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
            }
            else
            {
                TempData["Message"] = "User not found.";
            }
            return RedirectToAction("Index");
        }
    }
}