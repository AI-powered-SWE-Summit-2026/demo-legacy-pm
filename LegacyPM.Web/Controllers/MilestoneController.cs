using System;
using System.Linq;
using LegacyPM.Core;
using LegacyPM.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LegacyPM.Web.Controllers
{
    public class MilestoneController : Controller
    {
        private readonly ProjectDbContext _dbContext;

        public MilestoneController(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var milestones = _dbContext.Milestones
                .Include(m => m.Project)
                .OrderBy(m => m.DueDate)
                .ToList();
            return View(milestones);
        }

        public IActionResult Create()
        {
            ViewBag.Projects = new SelectList(_dbContext.Projects.ToList(), "Id", "Name");
            return View(new Milestone
            {
                DueDate = DateTime.Now.Date.AddDays(14)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Milestone milestone)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Projects = new SelectList(_dbContext.Projects.ToList(), "Id", "Name", milestone.ProjectId);
                return View(milestone);
            }

            _dbContext.Milestones.Add(milestone);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}