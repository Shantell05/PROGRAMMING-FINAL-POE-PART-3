using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using CybersecurityChatbotWPF.Models;

namespace CybersecurityChatbotWPF.Database
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;
        private bool _isConnected;

        public DatabaseHelper()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cybersecurity_tasks.db");
            _connectionString = $"Data Source={dbPath};Version=3;";
            _isConnected = false;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    _isConnected = true;
                    string query = @"CREATE TABLE IF NOT EXISTS Tasks (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        Description TEXT,
                        CreatedDate TEXT DEFAULT CURRENT_TIMESTAMP,
                        ReminderDate TEXT,
                        IsCompleted INTEGER DEFAULT 0
                    )";
                    using (var cmd = new SQLiteCommand(query, connection))
                        cmd.ExecuteNonQuery();
                }
            }
            catch { _isConnected = false; }
        }

        public bool IsConnected() => _isConnected;

        public bool AddTask(TaskItem task)
        {
            if (!_isConnected) return false;
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"INSERT INTO Tasks (Title, Description, ReminderDate, IsCompleted) 
                                    VALUES (@Title, @Description, @ReminderDate, @IsCompleted)";
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Title", task.Title);
                        cmd.Parameters.AddWithValue("@Description", task.Description ?? "");
                        cmd.Parameters.AddWithValue("@ReminderDate", task.ReminderDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "");
                        cmd.Parameters.AddWithValue("@IsCompleted", task.IsCompleted ? 1 : 0);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch { return false; }
        }

        public List<TaskItem> GetTasks()
        {
            var tasks = new List<TaskItem>();
            if (!_isConnected) return tasks;
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Tasks ORDER BY IsCompleted ASC, CreatedDate DESC";
                    using (var cmd = new SQLiteCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tasks.Add(new TaskItem
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Title = reader["Title"]?.ToString() ?? string.Empty,
                                Description = reader["Description"]?.ToString() ?? string.Empty,
                                CreatedDate = DateTime.TryParse(reader["CreatedDate"]?.ToString(), out DateTime created) ? created : DateTime.Now,
                                ReminderDate = DateTime.TryParse(reader["ReminderDate"]?.ToString(), out DateTime reminder) ? reminder : (DateTime?)null,
                                IsCompleted = Convert.ToInt32(reader["IsCompleted"]) == 1
                            });
                        }
                    }
                }
            }
            catch { }
            return tasks;
        }

        public bool UpdateTask(TaskItem task)
        {
            if (!_isConnected) return false;
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"UPDATE Tasks SET Title = @Title, Description = @Description, 
                                    ReminderDate = @ReminderDate, IsCompleted = @IsCompleted WHERE Id = @Id";
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", task.Id);
                        cmd.Parameters.AddWithValue("@Title", task.Title);
                        cmd.Parameters.AddWithValue("@Description", task.Description ?? "");
                        cmd.Parameters.AddWithValue("@ReminderDate", task.ReminderDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "");
                        cmd.Parameters.AddWithValue("@IsCompleted", task.IsCompleted ? 1 : 0);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch { return false; }
        }

        public bool DeleteTask(int taskId)
        {
            if (!_isConnected) return false;
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Tasks WHERE Id = @Id";
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", taskId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch { return false; }
        }

        public bool MarkTaskCompleted(int taskId)
        {
            if (!_isConnected) return false;
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Tasks SET IsCompleted = 1 WHERE Id = @Id";
                    using (var cmd = new SQLiteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", taskId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch { return false; }
        }
    }
}