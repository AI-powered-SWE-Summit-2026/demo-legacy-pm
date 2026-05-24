using System;
using System.Collections.Generic;

namespace LegacyPM.Core.Models
{
    public enum Priority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum ProjectTaskStatus
    {
        Todo,
        InProgress,
        Review,
        Done
    }

    public class ProjectTask
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? AssignedToId { get; set; }
        public DateTime DueDate { get; set; }
        public Priority Priority { get; set; }
        public ProjectTaskStatus Status { get; set; }
        public decimal EstimatedHours { get; set; }
        public decimal ActualHours { get; set; }
        public DateTime CreatedAt { get; set; }
        public Project Project { get; set; }
        public Resource AssignedTo { get; set; }
        public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
    }
}