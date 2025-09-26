using CarMaintenance.Data;
using CarMaintenance.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.IO;

namespace CarMaintenance.Controllers
{
    public class ReportsController : Controller
    {
        private readonly AppDbContext _db;

        public ReportsController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View("Search");
        }

        // Search method
        public IActionResult Search(string searchTerm)
        {
            searchTerm = searchTerm?.Trim() ?? "";

            // Load customers and transfers
            var customers = _db.Tbl_Customers
                .Include(c => c.Cars)
                .Include(c => c.Receipts)
                    .ThenInclude(r => r.ReceiptsDetails)
                        .ThenInclude(d => d.Services)
                .ToList();

            var transfers = _db.Tbl_TransferCars
                .Include(t => t.FromCustomers)
                .Include(t => t.ToCustomers)
                .Include(t => t.Cars)
                .ToList();

            // Filter customers
            var results = customers
                .Where(c =>
                    string.IsNullOrEmpty(searchTerm) ||
                    (c.Name != null && c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Email != null && c.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Cars != null && c.Cars.NumberPlate != null && c.Cars.NumberPlate.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                )
                .Select(c => new SearchResultViewModel
                {
                    Customer = c,
                    Transfers = transfers
                        .Where(t => t.FromCustomerID == c.CustomerID || t.ToCustomerID == c.CustomerID)
                        .ToList()
                })
                .ToList();

            // Logged-in user
            var userId = HttpContext.Session.GetInt32("UserId");
            string userName = "Unknown User";
            if (userId.HasValue)
            {
                var user = _db.Tbl_Users.Find(userId.Value);
                if (user != null)
                {
                    userName = $"{user.FirstName} {user.LastName}";
                }
            }

            ViewData["GeneratedBy"] = userName;
            ViewData["SearchTerm"] = searchTerm;

            return View("SearchResults", results);
        }

        // Autocomplete suggestions
        public IActionResult GetSearchSuggestions(string term)
        {
            term = term?.Trim() ?? "";

            var customers = _db.Tbl_Customers.Include(c => c.Cars).ToList();

            var suggestions = customers
                .SelectMany(c =>
                    new List<string>
                    {
                        c.Name ?? "",
                        c.Email ?? "",
                        c.Cars?.NumberPlate ?? ""
                    }
                )
                .Where(s => !string.IsNullOrEmpty(s) && s.Contains(term, StringComparison.OrdinalIgnoreCase))
                .Distinct()
                .Select(s => new { label = s, value = s })
                .Take(10)
                .ToList();

            return Json(suggestions);
        }

        // PDF Export
        public IActionResult ExportPdf(string searchTerm)
        {
            searchTerm = searchTerm?.Trim() ?? "";

            var customers = _db.Tbl_Customers
                .Include(c => c.Cars)
                .Include(c => c.Receipts)
                    .ThenInclude(r => r.ReceiptsDetails)
                        .ThenInclude(d => d.Services)
                .ToList();

            var transfers = _db.Tbl_TransferCars
                .Include(t => t.FromCustomers)
                .Include(t => t.ToCustomers)
                .Include(t => t.Cars)
                .ToList();

            var results = customers
                .Where(c =>
                    string.IsNullOrEmpty(searchTerm) ||
                    (c.Name != null && c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Email != null && c.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Cars != null && c.Cars.NumberPlate != null && c.Cars.NumberPlate.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                )
                .Select(c => new SearchResultViewModel
                {
                    Customer = c,
                    Transfers = transfers
                        .Where(t => t.FromCustomerID == c.CustomerID || t.ToCustomerID == c.CustomerID)
                        .ToList()
                })
                .ToList();

            var userId = HttpContext.Session.GetInt32("UserId");
            string userName = "Unknown User";
            if (userId.HasValue)
            {
                var user = _db.Tbl_Users.Find(userId.Value);
                if (user != null)
                    userName = $"{user.FirstName} {user.LastName}";
            }

            using (var ms = new MemoryStream())
            {
                var writer = new PdfWriter(ms);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                document.Add(new Paragraph("Customer Report").SetBold().SetFontSize(18));
                document.Add(new Paragraph($"Generated By: {userName}").SetFontSize(12));
                document.Add(new Paragraph($"Generated On: {DateTime.Now}").SetFontSize(12));
                document.Add(new Paragraph(" "));

                foreach (var r in results)
                {
                    document.Add(new Paragraph($"Customer: {r.Customer.Name} ({r.Customer.Email})").SetBold());
                    document.Add(new Paragraph($"Number Plate: {r.Customer.Cars?.NumberPlate ?? "N/A"}"));

                    document.Add(new Paragraph("Receipts:"));
                    foreach (var rec in r.Customer.Receipts)
                    {
                        document.Add(new Paragraph($"- Date: {rec.Date.ToShortDateString()}, Amount: {rec.TotalAmount:F2}"));
                        foreach (var d in rec.ReceiptsDetails)
                        {
                            document.Add(new Paragraph($"   • Service: {d.Services?.ServiceName ?? "N/A"}"));
                        }
                    }

                    document.Add(new Paragraph("Transfers:"));
                    foreach (var t in r.Transfers)
                    {
                        document.Add(new Paragraph(
                            $"- {t.FromCustomers?.Name ?? "N/A"} → {t.ToCustomers?.Name ?? "N/A"} on {t.TransferDate.ToShortDateString()}, Number Plate: {t.Cars?.NumberPlate ?? "N/A"}"));
                    }

                    document.Add(new Paragraph(" "));
                }

                document.Close();
                return File(ms.ToArray(), "application/pdf", "CustomerReport.pdf");
            }
        }
    }
}
