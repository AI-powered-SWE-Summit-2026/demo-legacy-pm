using System.Linq;
using LegacyPM.Core;
using LegacyPM.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace LegacyPM.Web.Controllers
{
    public class ResourceController : Controller
    {
        private readonly ProjectDbContext _dbContext;

        public ResourceController(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View(_dbContext.Resources.OrderBy(r => r.FullName).ToList());
        }

        public IActionResult Create()
        {
            return View(new Resource { IsActive = true, HourlyRate = 100M });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Resource resource)
        {
            if (!ModelState.IsValid)
            {
                return View(resource);
            }

            _dbContext.Resources.Add(resource);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var resource = _dbContext.Resources.FirstOrDefault(r => r.Id == id);
            if (resource == null)
            {
                return NotFound();
            }

            return View(resource);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Resource resource)
        {
            if (!ModelState.IsValid)
            {
                return View(resource);
            }

            var existing = _dbContext.Resources.FirstOrDefault(r => r.Id == resource.Id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.FullName = resource.FullName;
            existing.Email = resource.Email;
            existing.Role = resource.Role;
            existing.Department = resource.Department;
            existing.HourlyRate = resource.HourlyRate;
            existing.IsActive = resource.IsActive;

            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var resource = _dbContext.Resources.FirstOrDefault(r => r.Id == id);
            if (resource == null)
            {
                return NotFound();
            }

            var hasTimeEntries = _dbContext.TimeEntries.Any(t => t.ResourceId == id);
            var hasProjectMemberships = _dbContext.ProjectMembers.Any(m => m.ResourceId == id);

            if (hasTimeEntries || hasProjectMemberships)
            {
                resource.IsActive = false;
            }
            else
            {
                _dbContext.Resources.Remove(resource);
            }

            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}