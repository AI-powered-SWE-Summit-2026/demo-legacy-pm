using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LegacyPM.Core.Models;
using LegacyPM.Core.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LegacyPM.Core.Tests
{
    public class ProjectServiceTests
    {
        [Fact]
        public async Task SaveProjectAsync_NewProject_SetsTimestamps_AndPersists()
        {
            using var context = CreateDbContext();
            var service = new ProjectService(context);
            var project = new Project
            {
                Name = "Apollo",
                Description = "Migration project",
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 6, 30),
                Status = ProjectStatus.Planning,
                OwnerId = "owner-1",
                Budget = 10000m
            };

            await service.SaveProjectAsync(project);

            Assert.NotEqual(default, project.CreatedAt);
            Assert.NotEqual(default, project.UpdatedAt);
            Assert.Single(context.Projects);
        }

        [Fact]
        public void CalculateActualCost_ReturnsHourlyTotal_ForProjectTaskTimeEntries()
        {
            using var context = CreateDbContext();
            context.Projects.Add(new Project { Id = 1, Name = "Costed", Status = ProjectStatus.Active });
            context.ProjectTasks.AddRange(
                new ProjectTask { Id = 10, ProjectId = 1, Title = "Feature A", Status = ProjectTaskStatus.InProgress },
                new ProjectTask { Id = 11, ProjectId = 1, Title = "Feature B", Status = ProjectTaskStatus.Todo });
            context.Resources.Add(new Resource { Id = 1, FullName = "Dev 1", HourlyRate = 80m });
            context.TimeEntries.AddRange(
                new TimeEntry { ProjectTaskId = 10, ResourceId = 1, Hours = 2.5m, Date = DateTime.Today, Description = "Build" },
                new TimeEntry { ProjectTaskId = 11, ResourceId = 99, Hours = 4m, Date = DateTime.Today, Description = "No resource row" });
            context.SaveChanges();

            var service = new ProjectService(context);
            var project = new Project
            {
                Id = 1,
                Tasks = new List<ProjectTask>
                {
                    new ProjectTask { Id = 10 },
                    new ProjectTask { Id = 11 }
                }
            };

            var actualCost = service.CalculateActualCost(project);

            Assert.Equal(200m, actualCost);
        }

        private static ProjectDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ProjectDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ProjectDbContext(options);
        }
    }
}
