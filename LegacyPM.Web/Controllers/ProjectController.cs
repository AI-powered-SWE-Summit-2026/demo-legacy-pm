using System;
using LegacyPM.Core.Models;
using LegacyPM.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace LegacyPM.Web.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ProjectService _projectService;

        public ProjectController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        public IActionResult Index()
        {
            var projects = _projectService.GetAllProjectsAsync().Result;
            return View(projects);
        }

        public IActionResult Details(int id)
        {
            var project = _projectService.GetProjectAsync(id).Result;
            if (project == null)
            {
                return NotFound();
            }

            project.ActualCost = _projectService.CalculateActualCost(project);
            return View(project);
        }

        public IActionResult Create()
        {
            return View(new Project
            {
                StartDate = DateTime.Now.Date,
                EndDate = DateTime.Now.Date.AddDays(30),
                Status = ProjectStatus.Planning,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Project project)
        {
            if (!ModelState.IsValid)
            {
                return View(project);
            }

            project.CreatedAt = DateTime.Now;
            project.UpdatedAt = DateTime.Now;
            _projectService.SaveProjectAsync(project).Wait();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var project = _projectService.GetProjectAsync(id).Result;
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Project project)
        {
            if (!ModelState.IsValid)
            {
                return View(project);
            }

            project.UpdatedAt = DateTime.Now;
            _projectService.SaveProjectAsync(project).GetAwaiter().GetResult();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _projectService.DeleteProjectAsync(id).Wait();
            return RedirectToAction(nameof(Index));
        }
    }
}