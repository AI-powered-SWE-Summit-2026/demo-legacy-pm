using System;
using System.Collections.Generic;
using LegacyPM.Core.Models;

namespace LegacyPM.Web.Models
{
    public class DashboardViewModel
    {
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int OpenTasks { get; set; }
        public int ActiveResources { get; set; }
        public DateTime GeneratedAt { get; set; }
        public string ExternalStatus { get; set; }
        public List<Project> RecentProjects { get; set; } = new List<Project>();
    }
}