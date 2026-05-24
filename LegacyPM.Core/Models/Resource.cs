using System.Collections.Generic;

namespace LegacyPM.Core.Models
{
    public class Resource
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Department { get; set; }
        public decimal HourlyRate { get; set; }
        public bool IsActive { get; set; }
        public ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
        public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
        public ICollection<ProjectTask> AssignedTasks { get; set; } = new List<ProjectTask>();
    }
}