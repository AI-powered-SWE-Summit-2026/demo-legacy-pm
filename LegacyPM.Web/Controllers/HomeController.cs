using System;
using System.Linq;
using LegacyPM.Core;
using LegacyPM.Core.Models;
using LegacyPM.Core.Services;
using LegacyPM.Web.Models;
using LegacyPM.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegacyPM.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProjectDbContext _dbContext;
        private readonly ProjectService _projectService;
        private readonly ExternalHttpService _externalHttpService;

        public HomeController(ProjectDbContext dbContext, ProjectService projectService, ExternalHttpService externalHttpService)
        {
            _dbContext = dbContext;
            _projectService = projectService;
            _externalHttpService = externalHttpService;
        }

        public IActionResult Index()
        {
            var projects = _projectService.GetAllProjectsAsync().Result;
            var model = new DashboardViewModel
            {
                TotalProjects = projects.Count,
                ActiveProjects = projects.Count(p => p.Status == ProjectStatus.Active),
                OpenTasks = _dbContext.ProjectTasks.Count(t => t.Status != ProjectTaskStatus.Done),
                ActiveResources = _dbContext.Resources.Count(r => r.IsActive),
                GeneratedAt = DateTime.Now,
                RecentProjects = projects.Take(5).ToList(),
                ExternalStatus = "Dashboard ready"
            };

            try
            {
                var content = _externalHttpService.GetExternalData("https://example.com").Result;
                model.ExternalStatus = "External check length: " + content.Length;
            }
            catch
            {
                model.ExternalStatus = "External check unavailable at " + DateTime.Now;
            }

            ViewBag.UpcomingMilestones = _dbContext.Milestones
                .Include(m => m.Project)
                .Where(m => !m.IsCompleted)
                .OrderBy(m => m.DueDate)
                .Take(5)
                .ToList();

            return View(model);
        }
    }
}