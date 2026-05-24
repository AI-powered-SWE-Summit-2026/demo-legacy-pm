using System.Collections.Generic;
using LegacyPM.Core.Models;

namespace LegacyPM.Web.Models
{
    public class ReportGenerateViewModel
    {
        public int ProjectId { get; set; }
        public string ReportType { get; set; }
        public string Parameters { get; set; }
        public string GeneratedJson { get; set; }
        public IEnumerable<Project> Projects { get; set; } = new List<Project>();
    }
}