using System;
using LegacyPM.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LegacyPM.Core
{
    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<Milestone> Milestones { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<TimeEntry> TimeEntries { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<NotificationLog> NotificationLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .Property(p => p.Status)
                .HasConversion(v => v.ToString(), v => ParseProjectStatus(v));

            modelBuilder.Entity<ProjectTask>()
                .Property(t => t.Priority)
                .HasConversion(v => v.ToString(), v => ParsePriority(v));

            modelBuilder.Entity<ProjectTask>()
                .Property(t => t.Status)
                .HasConversion(v => v.ToString(), v => ParseProjectTaskStatus(v));

            modelBuilder.Entity<Project>()
                .Property(p => p.Budget)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Project>()
                .Property(p => p.ActualCost)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Resource>()
                .Property(r => r.HourlyRate)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ProjectTask>()
                .Property(t => t.EstimatedHours)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ProjectTask>()
                .Property(t => t.ActualHours)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<TimeEntry>()
                .Property(t => t.Hours)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Tasks)
                .WithOne(t => t.Project)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Milestones)
                .WithOne(m => m.Project)
                .HasForeignKey(m => m.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Members)
                .WithOne(m => m.Project)
                .HasForeignKey(m => m.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Reports)
                .WithOne(r => r.Project)
                .HasForeignKey(r => r.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectTask>()
                .HasOne(t => t.AssignedTo)
                .WithMany(r => r.AssignedTasks)
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TimeEntry>()
                .HasOne(t => t.ProjectTask)
                .WithMany(p => p.TimeEntries)
                .HasForeignKey(t => t.ProjectTaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TimeEntry>()
                .HasOne(t => t.Resource)
                .WithMany(r => r.TimeEntries)
                .HasForeignKey(t => t.ResourceId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProjectMember>()
                .HasOne(m => m.Resource)
                .WithMany(r => r.ProjectMembers)
                .HasForeignKey(m => m.ResourceId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<NotificationLog>()
                .Property(n => n.Culture)
                .HasMaxLength(10);
        }

        private static ProjectStatus ParseProjectStatus(string value) =>
            ParseEnumValue<ProjectStatus>(value, "project status");

        private static Priority ParsePriority(string value) =>
            ParseEnumValue<Priority>(value, "task priority");

        private static ProjectTaskStatus ParseProjectTaskStatus(string value) =>
            ParseEnumValue<ProjectTaskStatus>(value, "task status");

        private static TEnum ParseEnumValue<TEnum>(string value, string label) where TEnum : struct, Enum
        {
            var normalizedValue = (value ?? string.Empty)
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty)
                .Replace("_", string.Empty);

            if (Enum.TryParse<TEnum>(normalizedValue, true, out var parsed))
            {
                return parsed;
            }

            throw new ArgumentException($"Invalid {label} value '{value}' stored in the database.");
        }
    }
}