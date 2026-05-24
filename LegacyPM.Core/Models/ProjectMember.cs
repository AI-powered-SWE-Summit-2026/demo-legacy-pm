using System;

namespace LegacyPM.Core.Models
{
    public class ProjectMember
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int ResourceId { get; set; }
        public string Role { get; set; }
        public DateTime JoinedAt { get; set; }
        public Project Project { get; set; }
        public Resource Resource { get; set; }
    }
}