using System;
using System.Linq;
using System.Reflection;
using LegacyPM.Core;
using LegacyPM.Core.Models;
using LegacyPM.Core.Services;
using LegacyPM.Web.Models;
using LegacyPM.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace LegacyPM.Web.Controllers
{
    public class ReportController : Controller
    {
        private readonly ProjectDbContext _dbContext;
        private readonly ProjectService _projectService;
        private readonly ReportCacheService _reportCacheService;
        private readonly CultureService _cultureService;

        public ReportController(ProjectDbContext dbContext, ProjectService projectService, ReportCacheService reportCacheService, CultureService cultureService)
        {
            _dbContext = dbContext;
            _projectService = projectService;
            _reportCacheService = reportCacheService;
            _cultureService = cultureService;
        }

        public IActionResult Index()
        {
            var reports = _dbContext.Reports.OrderByDescending(r => r.GeneratedAt).ToList();
            return View(reports);
        }

        public IActionResult Generate()
        {
            var model = new ReportGenerateViewModel
            {
                Projects = _projectService.GetAllProjectsAsync().Result,
                ReportType = "StatusSummary"
            };
            ViewBag.Projects = new SelectList(model.Projects, "Id", "Name");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Generate(ReportGenerateViewModel model)
        {
            ViewBag.Projects = new SelectList(_projectService.GetAllProjectsAsync().Result, "Id", "Name", model.ProjectId);
            if (!ModelState.IsValid)
            {
                model.Projects = _projectService.GetAllProjectsAsync().Result;
                return View(model);
            }

            _cultureService.UseBrazilianCulture();
            var project = _projectService.GetProjectAsync(model.ProjectId).Result;
            if (project == null)
            {
                return NotFound();
            }

            var properties = typeof(Project).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var reportData = new
            {
                Project = project.Name,
                GeneratedAt = DateTime.Now,
                Parameters = model.Parameters,
                PropertyNames = properties.Select(p => p.Name).ToArray(),
                Tasks = project.Tasks.Select(t => new
                {
                    t.Title,
                    t.Status,
                    t.Priority,
                    DaysUntilDue = (t.DueDate - DateTime.Now).Days
                }).ToList(),
                Milestones = project.Milestones.Select(m => new
                {
                    m.Name,
                    m.DueDate,
                    m.IsCompleted
                }).ToList()
            };

            var report = new Report
            {
                ProjectId = project.Id,
                ReportType = model.ReportType,
                GeneratedAt = DateTime.Now,
                GeneratedByUserId = User.Identity != null && User.Identity.IsAuthenticated ? User.Identity.Name : "legacy-admin",
                CachedData = _reportCacheService.SerializeReportData(reportData),
                Parameters = model.Parameters
            };

            _dbContext.Reports.Add(report);
            _dbContext.SaveChanges();
            model.GeneratedJson = JsonConvert.SerializeObject(reportData, Formatting.Indented);
            model.Projects = _projectService.GetAllProjectsAsync().Result;
            return View(model);
        }
    }
}