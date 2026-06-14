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

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var entry = _dbContext.TimeEntries.Include(t => t.ProjectTask).Include(t => t.Resource).FirstOrDefault(t => t.Id == id);
            if (entry == null)
            {
                return NotFound();
            }

            return Ok(entry);
        }

        [HttpPost]
        public IActionResult Post([FromBody] TimeEntry timeEntry)
        {
            timeEntry.CreatedAt = System.DateTime.Now;
            _dbContext.TimeEntries.Add(timeEntry);
            _dbContext.SaveChanges();
            return Json(timeEntry);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] TimeEntry timeEntry)
        {
            var existing = _dbContext.TimeEntries.FirstOrDefault(t => t.Id == id);
            if (existing == null)
            {
                return NotFound();
            }

            existing.ProjectTaskId = timeEntry.ProjectTaskId;
            existing.ResourceId = timeEntry.ResourceId;
            existing.Date = timeEntry.Date;
            existing.Hours = timeEntry.Hours;
            existing.Description = timeEntry.Description;
            _dbContext.SaveChanges();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existing = _dbContext.TimeEntries.FirstOrDefault(t => t.Id == id);
            if (existing == null)
            {
                return NotFound();
            }

            _dbContext.TimeEntries.Remove(existing);
            _dbContext.SaveChanges();
            return NoContent();
        }
    }
}