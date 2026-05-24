using System;

namespace LegacyPM.Core.Models
{
    public class NotificationLog
    {
        public int Id { get; set; }
        public string RecipientEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime SentAt { get; set; }
        public string Status { get; set; }
        public string Culture { get; set; }
    }
}