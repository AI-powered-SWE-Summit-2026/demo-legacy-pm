using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacyPM.Core;
using LegacyPM.Core.Models;
using LegacyPM.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LegacyPM.Web.BackgroundServices
{
    public class DeadlineReminderBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DeadlineReminderBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await SendDeadlineReminders();
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task SendDeadlineReminders()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ProjectDbContext>();
            var emailReminderService = scope.ServiceProvider.GetRequiredService<EmailReminderService>();

            var tasks = await dbContext.ProjectTasks
                .Include(t => t.AssignedTo)
                .Include(t => t.Project)
                .Where(t => t.Status != ProjectTaskStatus.Done && t.DueDate <= DateTime.Now.AddDays(2))
                .ToListAsync();

            foreach (var task in tasks.Where(t => t.AssignedTo != null))
            {
                var culture = task.AssignedTo.Department == "LATAM" ? "pt-BR" : "en-US";
                emailReminderService.SendReminder(
                    task.AssignedTo,
                    "Upcoming deadline",
                    $"Task '{task.Title}' for project '{task.Project.Name}' is due on {task.DueDate}",
                    culture);
            }
        }
    }
}