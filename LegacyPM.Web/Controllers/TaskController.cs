using System;
using System.Linq;
using LegacyPM.Core;
using LegacyPM.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LegacyPM.Web.Controllers
{
    public class TaskController : Controller
    {
        private readonly ProjectDbContext _dbContext;

        public TaskController(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index(int? projectId)
        {
            ViewBag.Projects = new SelectList(_dbContext.Projects.OrderBy(p => p.Name).ToList(), "Id", "Name", projectId);
            var tasks = _dbContext.ProjectTasks
                .Include(t => t.Project)
                .Include(t => t.AssignedTo)
                .AsQueryable();

            if (projectId.HasValue)
            {
                tasks = tasks.Where(t => t.ProjectId == projectId.Value);
            }

            return View(tasks.OrderBy(t => t.DueDate).ToList());
        }

        public IActionResult Create()
        {
            PopulateLists();
            return View(new ProjectTask
            {
                DueDate = DateTime.Now.Date.AddDays(7),
                Status = ProjectTaskStatus.Todo,
                Priority = Priority.Medium,
                CreatedAt = DateTime.Now
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProjectTask task)
        {
            if (!ModelState.IsValid)
            {
                PopulateLists();
                return View(task);
            }

            task.CreatedAt = DateTime.Now;
            _dbContext.ProjectTasks.Add(task);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            PopulateLists();
            var task = _dbContext.ProjectTasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProjectTask task)
        {
            if (!ModelState.IsValid)
            {
                PopulateLists();
                return View(task);
            }

            var existing = _dbContext.ProjectTasks.FirstOrDefault(t => t.Id == task.Id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.ProjectId = task.ProjectId;
            existing.Title = task.Title;
            existing.Description = task.Description;
            existing.AssignedToId = task.AssignedToId;
            existing.DueDate = task.DueDate;
            existing.Priority = task.Priority;
            existing.Status = task.Status;
            existing.EstimatedHours = task.EstimatedHours;
            existing.ActualHours = task.ActualHours;
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private void PopulateLists()
        {
            ViewBag.Projects = new SelectList(_dbContext.Projects.ToListAsync().Result, "Id", "Name");
            ViewBag.Resources = new SelectList(_dbContext.Resources.Where(r => r.IsActive).ToListAsync().Result, "Id", "FullName");
        }
    }
}