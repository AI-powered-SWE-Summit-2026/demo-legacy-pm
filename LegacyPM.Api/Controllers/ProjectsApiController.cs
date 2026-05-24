using System.Linq;
using LegacyPM.Core;
using LegacyPM.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LegacyPM.Api.Controllers
{
    [Route("api/projects")]
    public class ProjectsApiController : Controller
    {
        private readonly ProjectDbContext _dbContext;

        public ProjectsApiController(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var projects = _dbContext.Projects.Include(p => p.Tasks).ToList();
            var json = System.Text.Json.JsonSerializer.Serialize(projects.Select(p => new { p.Id, p.Name, p.Status }));
            Response.Headers["X-Project-Preview-Length"] = json.Length.ToString();
            return Json(projects);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var project = _dbContext.Projects.Include(p => p.Tasks).FirstOrDefault(p => p.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Project project)
        {
            project.CreatedAt = System.DateTime.Now;
            project.UpdatedAt = System.DateTime.Now;
            _dbContext.Projects.Add(project);
            _dbContext.SaveChanges();
            return Ok(project);
        }
    }
}