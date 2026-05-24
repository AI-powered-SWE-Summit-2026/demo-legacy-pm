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
    }
}