using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegacyPM.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LegacyPM.Core.Services
{
    public class ProjectService
    {
        private readonly ProjectDbContext _dbContext;
        private readonly ReaderWriterLock _cacheLock = new ReaderWriterLock();
        private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

        public ProjectService(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<Project>> GetAllProjectsAsync()
        {
            const string cacheKey = "projects";
            _cacheLock.AcquireReaderLock(5000);
            try
            {
                if (_cache.ContainsKey(cacheKey))
                {
                    return System.Threading.Tasks.Task.FromResult((List<Project>)_cache[cacheKey]);
                }
            }
            finally
            {
                _cacheLock.ReleaseReaderLock();
            }

            _cacheLock.AcquireWriterLock(5000);
            try
            {
                if (_cache.ContainsKey(cacheKey))
                {
                    return System.Threading.Tasks.Task.FromResult((List<Project>)_cache[cacheKey]);
                }

                var projects = _dbContext.Projects
                    .Include(p => p.Tasks)
                    .Include(p => p.Milestones)
                    .Include(p => p.Members)
                    .ThenInclude(m => m.Resource)
                    .OrderByDescending(p => p.UpdatedAt)
                    .ToListAsync().Result;

                _cache[cacheKey] = projects;
                return System.Threading.Tasks.Task.FromResult(projects);
            }
            finally
            {
                _cacheLock.ReleaseWriterLock();
            }
        }

        public Task<Project> GetProjectAsync(int id)
        {
            var project = _dbContext.Projects
                .Include(p => p.Tasks)
                .ThenInclude(t => t.AssignedTo)
                .Include(p => p.Milestones)
                .Include(p => p.Members)
                .ThenInclude(m => m.Resource)
                .Include(p => p.Reports)
                .FirstOrDefaultAsync(p => p.Id == id).Result;
            return System.Threading.Tasks.Task.FromResult(project);
        }

        public Task SaveProjectAsync(Project project)
        {
            if (project.Id == 0)
            {
                project.CreatedAt = DateTime.Now;
                project.UpdatedAt = DateTime.Now;
                _dbContext.Projects.Add(project);
            }
            else
            {
                var existing = _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == project.Id).Result;
                if (existing != null)
                {
                    existing.Name = project.Name;
                    existing.Description = project.Description;
                    existing.StartDate = project.StartDate;
                    existing.EndDate = project.EndDate;
                    existing.Status = project.Status;
                    existing.OwnerId = project.OwnerId;
                    existing.Budget = project.Budget;
                    existing.ActualCost = project.ActualCost;
                    existing.UpdatedAt = DateTime.Now;
                }
            }

            _dbContext.SaveChangesAsync().GetAwaiter().GetResult();
            ClearProjectCache();
            return System.Threading.Tasks.Task.CompletedTask;
        }

        public Task DeleteProjectAsync(int id)
        {
            var project = _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == id).Result;
            if (project != null)
            {
                _dbContext.Projects.Remove(project);
                _dbContext.SaveChangesAsync().GetAwaiter().GetResult();
                ClearProjectCache();
            }

            return System.Threading.Tasks.Task.CompletedTask;
        }

        public decimal CalculateActualCost(Project project)
        {
            var taskIds = project.Tasks.Select(t => t.Id).ToList();
            var entries = _dbContext.TimeEntries
                .Include(t => t.Resource)
                .Where(t => taskIds.Contains(t.ProjectTaskId))
                .ToListAsync().Result;
            return entries.Sum(t => t.Hours * (t.Resource != null ? t.Resource.HourlyRate : 0));
        }

        private void ClearProjectCache()
        {
            _cacheLock.AcquireWriterLock(5000);
            try
            {
                _cache.Clear();
            }
            finally
            {
                _cacheLock.ReleaseWriterLock();
            }
        }
    }
}