 using System;
using System.Collections.Generic;
using System.Linq;
using CybersecurityChatbotWPF.Models;
using CybersecurityChatbotWPF.Database;

namespace CybersecurityChatbotWPF.Services
{
    public class TaskManager
    {
        private readonly DatabaseHelper _database;
        private List<TaskItem> _tasks;
        private bool _useDatabase;

        public TaskManager()
        {
            _database = new DatabaseHelper();
            _useDatabase = _database.IsConnected();
            _tasks = _useDatabase ? _database.GetTasks() : new List<TaskItem>();
        }

        public List<TaskItem> Tasks => _tasks;

        public bool AddTask(TaskItem task)
        {
            if (_useDatabase)
            {
                if (_database.AddTask(task))
                {
                    _tasks = _database.GetTasks();
                    return true;
                }
                return false;
            }
            task.Id = _tasks.Count > 0 ? _tasks.Max(t => t.Id) + 1 : 1;
            _tasks.Add(task);
            return true;
        }

        public List<TaskItem> GetTasks() => _useDatabase ? _database.GetTasks() : _tasks;

        public bool UpdateTask(TaskItem task)
        {
            if (_useDatabase) return _database.UpdateTask(task);
            var existing = _tasks.FirstOrDefault(t => t.Id == task.Id);
            if (existing != null)
            {
                existing.Title = task.Title;
                existing.Description = task.Description;
                existing.ReminderDate = task.ReminderDate;
                existing.IsCompleted = task.IsCompleted;
                return true;
            }
            return false;
        }

        public bool DeleteTask(int taskId)
        {
            if (_useDatabase)
            {
                if (_database.DeleteTask(taskId))
                {
                    _tasks = _database.GetTasks();
                    return true;
                }
                return false;
            }
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null) { _tasks.Remove(task); return true; }
            return false;
        }

        public bool MarkTaskCompleted(int taskId)
        {
            if (_useDatabase)
            {
                if (_database.MarkTaskCompleted(taskId))
                {
                    _tasks = _database.GetTasks();
                    return true;
                }
                return false;
            }
            var task = _tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null) { task.IsCompleted = true; return true; }
            return false;
        }
    }
}
