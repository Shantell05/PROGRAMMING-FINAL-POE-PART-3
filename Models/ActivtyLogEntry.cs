 using System;

namespace CybersecurityChatbotWPF.Models
{
    public class ActivityLogEntry
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Action { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        public string DisplayText => $"{Timestamp:HH:mm:ss} - {Action}: {Details}";
    }
}
