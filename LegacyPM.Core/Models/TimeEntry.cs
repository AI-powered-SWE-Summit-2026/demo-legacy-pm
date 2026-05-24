using System;

namespace LegacyPM.Core.Models
{
    public class TimeEntry
    {
        public int Id { get; set; }
        public int ProjectTaskId { get; set; }
        public int ResourceId { get; set; }
        public DateTime Date { get; set; }
        public decimal Hours { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public ProjectTask ProjectTask { get; set; }
        public Resource Resource { get; set; }
    }
}