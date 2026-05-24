using System;
using System.Collections.Generic;

namespace LegacyPM.Core.Models
{
    public enum ProjectStatus
    {
        Planning,
        Active,
        OnHold,
        Completed,
        Cancelled
    }

    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ProjectStatus Status { get; set; }
        public string OwnerId { get; set; }
        public decimal Budget { get; set; }
        public decimal ActualCost { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
        public ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();
        public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}