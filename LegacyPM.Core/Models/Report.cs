using System;

namespace LegacyPM.Core.Models
{
    [Serializable]
    public class Report
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ReportType { get; set; }
        public DateTime GeneratedAt { get; set; }
        public string GeneratedByUserId { get; set; }
        public byte[] CachedData { get; set; }
        public string Parameters { get; set; }
        public Project Project { get; set; }
    }
}