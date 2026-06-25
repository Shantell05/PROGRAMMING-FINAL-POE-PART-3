using System;
using System.Collections.Generic;
using System.Linq;
using CybersecurityChatbotWPF.Models;

namespace CybersecurityChatbotWPF.Services
{
    /// <summary>
    /// Manages the activity log for tracking bot actions
    /// </summary>
    public class ActivityLogService
    {
        private List<ActivityLogEntry> _logs;
        private readonly int _maxDisplayCount = 10;
        private int _currentPage = 0;

        public ActivityLogService()
        {
            _logs = new List<ActivityLogEntry>();
        }

        /// <summary>
        /// Adds an entry to the activity log
        /// </summary>
        public void AddLogEntry(string action, string details, string category = "General")
        {
            _logs.Add(new ActivityLogEntry
            {
                Timestamp = DateTime.Now,
                Action = action,
                Details = details,
                Category = category
            });
        }

        /// <summary>
        /// Gets all log entries
        /// </summary>
        public List<ActivityLogEntry> GetAllLogs()
        {
            return _logs.OrderByDescending(l => l.Timestamp).ToList();
        }

        /// <summary>
        /// Gets the most recent log entries
        /// </summary>
        public List<ActivityLogEntry> GetRecentLogs(int count = 10)
        {
            return _logs.OrderByDescending(l => l.Timestamp).Take(count).ToList();
        }

        /// <summary>
        /// Gets log entries for display (with pagination)
        /// </summary>
        public List<ActivityLogEntry> GetLogsForDisplay()
        {
            return _logs.OrderByDescending(l => l.Timestamp)
                       .Skip(_currentPage * _maxDisplayCount)
                       .Take(_maxDisplayCount)
                       .ToList();
        }

        /// <summary>
        /// Gets formatted log summary
        /// </summary>
        public string GetLogSummary()
        {
            if (_logs.Count == 0)
                return "No activities have been logged yet.";

            var recent = GetRecentLogs(10);
            string summary = "📋 Recent Activity Log:\n\n";

            for (int i = 0; i < recent.Count; i++)
            {
                var log = recent[i];
                summary += $"{i + 1}. {log.Timestamp:HH:mm:ss} - {log.Action}: {log.Details}\n";
            }

            if (_logs.Count > 10)
            {
                summary += $"\n... and {_logs.Count - 10} more entries";
            }

            return summary;
        }

        /// <summary>
        /// Gets log count
        /// </summary>
        public int Count => _logs.Count;

        /// <summary>
        /// Clears all logs
        /// </summary>
        public void ClearLogs()
        {
            _logs.Clear();
        }

        /// <summary>
        /// Logs task-related actions
        /// </summary>
        public void LogTaskAction(string action, string taskTitle)
        {
            AddLogEntry($"Task {action}", taskTitle, "Task");
        }

        /// <summary>
        /// Logs quiz actions
        /// </summary>
        public void LogQuizAction(string action, string details)
        {
            AddLogEntry($"Quiz {action}", details, "Quiz");
        }

        /// <summary>
        /// Logs NLP actions
        /// </summary>
        public void LogNLPAction(string action, string input)
        {
            AddLogEntry($"NLP: {action}", input, "NLP");
        }

        /// <summary>
        /// Logs reminder actions
        /// </summary>
        public void LogReminderAction(string action, string reminderDetails)
        {
            AddLogEntry($"Reminder {action}", reminderDetails, "Reminder");
        }
    }
}