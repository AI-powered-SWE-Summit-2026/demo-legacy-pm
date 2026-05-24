using System.Linq;
using LegacyPM.Core;
using LegacyPM.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LegacyPM.Api.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TasksApiController : ControllerBase
    {
        private readonly ProjectDbContext _dbContext;

        public TasksApiController(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var tasks = _dbContext.ProjectTasks.Include(t => t.Project).Include(t => t.AssignedTo).ToList();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var task = _dbContext.ProjectTasks.Include(t => t.Project).Include(t => t.AssignedTo).FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            var json2 = JsonConvert.SerializeObject(task);
            Response.Headers["X-Task-Preview-Length"] = json2.Length.ToString();
            return Ok(task);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProjectTask task)
        {
            task.CreatedAt = System.DateTime.Now;
            _dbContext.ProjectTasks.Add(task);
            _dbContext.SaveChanges();
            return Ok(task);
        }
    }
}