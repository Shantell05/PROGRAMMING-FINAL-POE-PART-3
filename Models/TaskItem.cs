 using System;

namespace CybersecurityChatbotWPF.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }

        public string Status => IsCompleted ? "✅ Completed" : "⏳ Pending";
        public string ReminderDisplay => ReminderDate.HasValue ? $"Reminder: {ReminderDate.Value:yyyy-MM-dd}" : "No reminder";
    }
}
