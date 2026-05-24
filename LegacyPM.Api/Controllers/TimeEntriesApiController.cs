using System.Linq;
using LegacyPM.Core;
using LegacyPM.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegacyPM.Api.Controllers
{
    [Route("api/timeentries")]
    public class TimeEntriesApiController : Controller
    {
        private readonly ProjectDbContext _dbContext;

        public TimeEntriesApiController(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var entries = _dbContext.TimeEntries.Include(t => t.ProjectTask).Include(t => t.Resource).ToList();
            return Ok(entries);
        }

        [HttpPost]
        public IActionResult Post([FromBody] TimeEntry timeEntry)
        {
            timeEntry.CreatedAt = System.DateTime.Now;
            _dbContext.TimeEntries.Add(timeEntry);
            _dbContext.SaveChanges();
            return Json(timeEntry);
        }
    }
}