using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KizspyWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KizspyWebApp.Controllers
{
    [Area("Database")]
    [Route("/database-manage/[action]")]
    public class DbManageController : Controller
    {
        private readonly KizspyDbContext _context;
        public DbManageController(KizspyDbContext dbContext)
        {
            _context = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DeleteDb()
        {
            return View();
        }

        [TempData]
        public string StatusMessage { get; set; }
        [HttpPost]
        public async Task<IActionResult> DeleteDbAsync()
        {
            var success = await _context.Database.EnsureDeletedAsync();
            StatusMessage = success ? "Delete database successfully" : "Delete database failed";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Migrate()
        {
            await _context.Database.MigrateAsync();
            StatusMessage = "Migrate database successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}