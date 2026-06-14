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

        public IActionResult Edit(int id)
        {
            var milestone = _dbContext.Milestones.FirstOrDefault(m => m.Id == id);
            if (milestone == null)
            {
                return NotFound();
            }

            ViewBag.Projects = new SelectList(_dbContext.Projects.ToList(), "Id", "Name", milestone.ProjectId);
            return View(milestone);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Milestone milestone)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Projects = new SelectList(_dbContext.Projects.ToList(), "Id", "Name", milestone.ProjectId);
                return View(milestone);
            }

            var existing = _dbContext.Milestones.FirstOrDefault(m => m.Id == milestone.Id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.ProjectId = milestone.ProjectId;
            existing.Name = milestone.Name;
            existing.Description = milestone.Description;
            existing.DueDate = milestone.DueDate;
            existing.IsCompleted = milestone.IsCompleted;
            existing.CompletedAt = milestone.IsCompleted
                ? existing.CompletedAt ?? DateTime.Now
                : null;

            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var milestone = _dbContext.Milestones.FirstOrDefault(m => m.Id == id);
            if (milestone != null)
            {
                _dbContext.Milestones.Remove(milestone);
                _dbContext.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}