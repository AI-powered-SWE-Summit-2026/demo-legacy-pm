using System;
using System.Linq;
using LegacyPM.Core;
using LegacyPM.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LegacyPM.Web.Controllers
{
    public class TimeEntryController : Controller
    {
        private readonly ProjectDbContext _dbContext;

        public TimeEntryController(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var entries = _dbContext.TimeEntries
                .Include(t => t.ProjectTask)
                .ThenInclude(t => t.Project)
                .Include(t => t.Resource)
                .OrderByDescending(t => t.Date)
                .ToList();
            return View(entries);
        }

        public IActionResult Create()
        {
            PopulateLists();
            return View(new TimeEntry
            {
                Date = DateTime.Now.Date,
                Hours = 1,
                CreatedAt = DateTime.Now
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TimeEntry timeEntry)
        {
            if (!ModelState.IsValid)
            {
                PopulateLists();
                return View(timeEntry);
            }

            timeEntry.CreatedAt = DateTime.Now;
            _dbContext.TimeEntries.Add(timeEntry);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private void PopulateLists()
        {
            ViewBag.Tasks = new SelectList(_dbContext.ProjectTasks.OrderBy(t => t.Title).ToList(), "Id", "Title");
            ViewBag.Resources = new SelectList(_dbContext.Resources.OrderBy(r => r.FullName).ToList(), "Id", "FullName");
        }
    }
}