using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CybersecurityChatbotWPF.Models;
using CybersecurityChatbotWPF.Services;

namespace CybersecurityChatbotWPF
{
    public partial class MainWindow : Window
    {
        private readonly ChatbotService _chatbotService;
        private readonly AudioService _audioService;
        private readonly TaskManager _taskManager;
        private readonly QuizManager _quizManager;
        private readonly NLPService _nlpService;
        private readonly ActivityLogService _activityLog;
        private ObservableCollection<ChatMessage> _messages;
        private bool _isQuizActive = false;

        public MainWindow()
        {
            InitializeComponent();

            LoadAsciiArt();

            _chatbotService = new ChatbotService();
            _audioService = new AudioService();
            _taskManager = new TaskManager();
            _quizManager = new QuizManager();
            _nlpService = new NLPService();
            _activityLog = new ActivityLogService();
            _messages = new ObservableCollection<ChatMessage>();

            MessagesItemsControl.ItemsSource = _messages;
            TasksListControl.ItemsSource = _taskManager.GetTasks();
            ActivityLogControl.ItemsSource = _activityLog.GetRecentLogs();

            _activityLog.AddLogEntry("Application Started", "Cybersecurity Chatbot initialized", "System");

            _audioService.PlayVoiceGreeting();

            AddBotMessage("🛡️ Welcome to the Cybersecurity Awareness Bot!");
            AddBotMessage("I'm here to help you stay safe online.");
            AddBotMessage("💡 New features: Task Manager, Cybersecurity Quiz, Activity Log!");
            AddBotMessage("Please enter your name to get started!");

            UpdateTaskCount();
        }

        private void LoadAsciiArt()
        {
            string asciiArt = @"
╔════════════════════════════════════════════════════════════════════════════╗
║                     CYBERSECURITY AWARENESS BOT - PART 3                   ║
╠════════════════════════════════════════════════════════════════════════════╣
║  ██████╗██╗   ██╗██████╗ ███████╗██████╗     █████╗ ██╗    ██╗ █████╗ ██████╗║
║ ██╔════╝╚██╗ ██╔╝██╔══██╗██╔════╝██╔══██╗   ██╔══██╗██║    ██║██╔══██╗██╔══██╗║
║ ██║      ╚████╔╝ ██████╔╝█████╗  ██████╔╝   ███████║██║ █╗ ██║███████║██████╔╝║
║ ██║       ╚██╔╝  ██╔══██╗██╔══╝  ██╔══██╗   ██╔══██║██║███╗██║██╔══██║██╔══██╗║
║ ╚██████╗   ██║   ██████╔╝███████╗██║  ██║   ██║  ██║╚███╔███╔╝██║  ██║██║  ██║║
║  ╚═════╝   ╚═╝   ╚═════╝ ╚══════╝╚═╝  ╚═╝   ╚═╝  ╚═╝ ╚══╝╚══╝ ╚═╝  ╚═╝╚═╝  ╚═╝║
╠════════════════════════════════════════════════════════════════════════════╣
║           Task Assistant | Quiz Game | NLP | Activity Log                  ║
╚════════════════════════════════════════════════════════════════════════════╝";

            AsciiArtTextBlock.Text = asciiArt;
        }

        private void AddUserMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                _messages.Add(new ChatMessage
                {
                    Text = message,
                    IsUser = true,
                    Timestamp = DateTime.Now,
                    Alignment = HorizontalAlignment.Right,
                    BubbleColor = new SolidColorBrush(Color.FromRgb(26, 95, 122))
                });
                ScrollToBottom();
            });
        }

        private void AddBotMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                _messages.Add(new ChatMessage
                {
                    Text = message,
                    IsUser = false,
                    Timestamp = DateTime.Now,
                    Alignment = HorizontalAlignment.Left,
                    BubbleColor = new SolidColorBrush(Color.FromRgb(45, 45, 45))
                });
                ScrollToBottom();
            });
        }

        private void ScrollToBottom()
        {
            Dispatcher.Invoke(() =>
            {
                ChatScrollViewer.UpdateLayout();
                ChatScrollViewer.ScrollToBottom();
            });
        }

        private async Task ShowTypingIndicator(bool show)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                TypingIndicator.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Exit Confirmation",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await ProcessUserInput();
        }

        private async void InputTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(InputTextBox.Text))
            {
                await ProcessUserInput();
            }
        }

        private async void StatsButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowTypingIndicator(true);
            await Task.Delay(300);

            string stats = _chatbotService.GetStatistics();
            AddBotMessage(stats);

            if (_chatbotService.Memory.HasName)
            {
                AddBotMessage($"I remember your name is {_chatbotService.Memory.RecallName()}!");
            }

            if (_chatbotService.Memory.HasPreferredTopic)
            {
                AddBotMessage($"I've noticed you're interested in {_chatbotService.Memory.RecallPreferredTopic()}. Great focus area!");
            }

            await ShowTypingIndicator(false);
        }

        private async void SaveNameButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please enter a valid name (at least 2 characters).", "Input Required",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (name.Length < 2)
            {
                MessageBox.Show("Name must be at least 2 characters long.", "Invalid Name",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (name.Length > 30)
            {
                MessageBox.Show("Name must be less than 30 characters.", "Invalid Name",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _chatbotService.SetUserName(name);
            NameInputPanel.Visibility = Visibility.Collapsed;

            InputTextBox.IsEnabled = true;
            SendButton.IsEnabled = true;
            HelpButton.IsEnabled = true;

            InputTextBox.Focus();

            await ShowTypingIndicator(true);
            await Task.Delay(500);

            string welcome = _chatbotService.GetPersonalizedWelcome();
            AddBotMessage(welcome);

            await Task.Delay(300);
            AddBotMessage("You can ask me about passwords, phishing, privacy, 2FA, safe browsing, malware, or VPNs!");
            AddBotMessage("Type any cybersecurity question - I'll remember what you're interested in!");

            await ShowTypingIndicator(false);
        }

        private async void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowTypingIndicator(true);
            await Task.Delay(300);

            AddBotMessage("I can help you with these cybersecurity topics:\n\n" +
                "Password Safety - Ask about 'password' or 'password tips'\n" +
                "Phishing Detection - Ask about 'phishing' or 'scam'\n" +
                "Privacy Protection - Ask about 'privacy'\n" +
                "Two-Factor Authentication - Ask about '2fa'\n" +
                "Safe Browsing - Ask about 'browsing'\n" +
                "Malware Protection - Ask about 'malware'\n" +
                "VPN Security - Ask about 'vpn'\n\n" +
                "Try these phrases:\n" +
                "- 'Give me a phishing tip'\n" +
                "- 'Tell me another tip'\n" +
                "- 'How do I create a strong password?'\n" +
                "- 'What is 2FA and why do I need it?'\n\n" +
                "I remember your interests! You can ask for 'another tip' anytime!");

            await ShowTypingIndicator(false);
        }

        private async Task ProcessUserInput()
        {
            string userInput = InputTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(userInput))
                return;

            InputTextBox.IsEnabled = false;
            SendButton.IsEnabled = false;

            AddUserMessage(userInput);
            InputTextBox.Clear();

            await ShowTypingIndicator(true);

            string response = await _chatbotService.ProcessUserInput(userInput);

            await ShowTypingIndicator(false);

            AddBotMessage(response);
            UpdateStatusFromSentiment(userInput);

            InputTextBox.IsEnabled = true;
            SendButton.IsEnabled = true;
            InputTextBox.Focus();
        }

        private void UpdateStatusFromSentiment(string userInput)
        {
            string lowerInput = userInput.ToLower();

            if (lowerInput.Contains("worried") || lowerInput.Contains("scared") || lowerInput.Contains("nervous"))
            {
                StatusText.Text = "User seems worried - Providing reassurance...";
                StatusText.Foreground = new SolidColorBrush(Colors.Orange);
                SentimentEmoji.Text = "😟";
                SentimentText.Text = "Worried";
                SentimentText.Foreground = new SolidColorBrush(Colors.Orange);
            }
            else if (lowerInput.Contains("frustrated") || lowerInput.Contains("confused"))
            {
                StatusText.Text = "User frustrated - Offering simplified help...";
                StatusText.Foreground = new SolidColorBrush(Colors.Red);
                SentimentEmoji.Text = "😤";
                SentimentText.Text = "Frustrated";
                SentimentText.Foreground = new SolidColorBrush(Colors.Red);
            }
            else if (lowerInput.Contains("curious") || lowerInput.Contains("interested") || lowerInput.Contains("tell me"))
            {
                StatusText.Text = "User curious - Providing educational content...";
                StatusText.Foreground = new SolidColorBrush(Colors.LightBlue);
                SentimentEmoji.Text = "🤔";
                SentimentText.Text = "Curious";
                SentimentText.Foreground = new SolidColorBrush(Colors.LightBlue);
            }
            else if (lowerInput.Contains("thanks") || lowerInput.Contains("good") || lowerInput.Contains("great"))
            {
                StatusText.Text = "Positive interaction - User satisfied...";
                StatusText.Foreground = new SolidColorBrush(Colors.LightGreen);
                SentimentEmoji.Text = "😊";
                SentimentText.Text = "Positive";
                SentimentText.Foreground = new SolidColorBrush(Colors.LightGreen);
            }
            else
            {
                StatusText.Text = "Ready to help with cybersecurity!";
                StatusText.Foreground = new SolidColorBrush(Colors.LightGreen);
                SentimentEmoji.Text = "😐";
                SentimentText.Text = "Neutral";
                SentimentText.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        // ===== TASK METHODS =====

        private void UpdateTaskCount()
        {
            var tasks = _taskManager.GetTasks();
            TaskCountLabel.Text = $"Total: {tasks.Count} tasks | Pending: {tasks.Count(t => !t.IsCompleted)}";
        }

        private void RefreshTaskList()
        {
            TasksListControl.ItemsSource = null;
            TasksListControl.ItemsSource = _taskManager.GetTasks();
            UpdateTaskCount();
        }

        private async void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TaskTitleBox.Text.Trim();
            string description = TaskDescriptionBox.Text.Trim();
            string reminderText = TaskReminderBox.Text.Trim();
            int days = 0;

            if (string.IsNullOrWhiteSpace(title) || title == "Enter task title...")
            {
                MessageBox.Show("Please enter a task title.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var task = new TaskItem
            {
                Title = title,
                Description = string.IsNullOrWhiteSpace(description) || description == "Enter task description..." ? "No description" : description
            };

            if (!string.IsNullOrWhiteSpace(reminderText) && int.TryParse(reminderText, out days) && days > 0)
            {
                task.ReminderDate = DateTime.Now.AddDays(days);
            }

            if (_taskManager.AddTask(task))
            {
                _activityLog.LogTaskAction("Added", task.Title);
                AddBotMessage($"✅ Task added: '{task.Title}' {(task.ReminderDate.HasValue ? $"with reminder in {days} days" : "")}");

                TaskTitleBox.Text = "Enter task title...";
                TaskDescriptionBox.Text = "Enter task description...";
                TaskReminderBox.Text = "Reminder (days)...";
                RefreshTaskList();
            }
            else
            {
                AddBotMessage("❌ Could not add task. Database may be unavailable.");
            }
        }

        private void CompleteTask_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is TaskItem task)
            {
                if (_taskManager.MarkTaskCompleted(task.Id))
                {
                    _activityLog.LogTaskAction("Completed", task.Title);
                    AddBotMessage($"✅ Task marked as completed: '{task.Title}'");
                    RefreshTaskList();
                }
            }
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is TaskItem task)
            {
                var result = MessageBox.Show($"Delete task '{task.Title}'?", "Confirm Delete",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (_taskManager.DeleteTask(task.Id))
                    {
                        _activityLog.LogTaskAction("Deleted", task.Title);
                        AddBotMessage($"🗑️ Task deleted: '{task.Title}'");
                        RefreshTaskList();
                    }
                }
            }
        }

        // ===== QUIZ METHODS =====

        private void StartQuizButton_Click(object sender, RoutedEventArgs e)
        {
            _quizManager.StartQuiz();
            _isQuizActive = true;

            _activityLog.LogQuizAction("Started", $"Quiz with {_quizManager.TotalQuestions} questions");

            QuizTitleText.Text = "🎯 Cybersecurity Quiz";
            QuizProgressText.Text = $"Question 1 of {_quizManager.TotalQuestions}";
            StartQuizButton.Visibility = Visibility.Collapsed;
            QuizQuestionPanel.Visibility = Visibility.Visible;
            QuizCompleteText.Text = "";

            ShowQuizQuestion();
        }

        private void ShowQuizQuestion()
        {
            var question = _quizManager.CurrentQuestion;
            if (question == null) return;

            QuizQuestionText.Text = question.Question;
            QuizOptionsControl.ItemsSource = question.Options;
            QuizFeedbackText.Text = "";
            QuizProgressText.Text = $"Question {_quizManager.CurrentQuestionNumber} of {_quizManager.TotalQuestions} | Score: {_quizManager.Score}";
        }

        private async void QuizOption_Click(object sender, RoutedEventArgs e)
        {
            if (!_isQuizActive || _quizManager.CurrentQuestion == null) return;

            var button = sender as Button;
            if (button == null) return;

            string buttonText = button.Content?.ToString() ?? "";
            int selectedIndex = _quizManager.CurrentQuestion.Options.IndexOf(buttonText);

            if (selectedIndex < 0) return;

            string feedback = _quizManager.SubmitAnswer(selectedIndex);
            QuizFeedbackText.Text = feedback;
            QuizFeedbackText.Foreground = new SolidColorBrush(Colors.LightGreen);

            _activityLog.LogQuizAction("Answered", $"{_quizManager.CurrentQuestionNumber - 1}/{_quizManager.TotalQuestions}");

            QuizOptionsControl.IsEnabled = false;

            await Task.Delay(1500);

            if (_quizManager.IsQuizActive)
            {
                QuizOptionsControl.IsEnabled = true;
                ShowQuizQuestion();
            }
            else
            {
                QuizQuestionPanel.Visibility = Visibility.Collapsed;
                QuizCompleteText.Text = $"🏆 Quiz Complete!\nScore: {_quizManager.Score}/{_quizManager.TotalQuestions}\n\n{GetScoreFeedback(_quizManager.Score, _quizManager.TotalQuestions)}";
                QuizCompleteText.Visibility = Visibility.Visible;
                StartQuizButton.Visibility = Visibility.Visible;
                StartQuizButton.Content = "🔄 Retake Quiz";
                _isQuizActive = false;

                _activityLog.LogQuizAction("Completed", $"Score: {_quizManager.Score}/{_quizManager.TotalQuestions}");

                AddBotMessage($"🎯 Quiz completed! Score: {_quizManager.Score}/{_quizManager.TotalQuestions}");
            }
        }

        private string GetScoreFeedback(int score, int total)
        {
            double percentage = (double)score / total * 100;
            if (percentage >= 90) return "🌟 Excellent! You're a cybersecurity pro!";
            if (percentage >= 70) return "👍 Good job! Keep learning to become a pro!";
            if (percentage >= 50) return "📚 Not bad! Review the topics you missed!";
            return "💪 Keep learning! Cybersecurity is important for everyone!";
        }

        // ===== ACTIVITY LOG METHODS =====

        private void RefreshLogDisplay()
        {
            ActivityLogControl.ItemsSource = null;
            ActivityLogControl.ItemsSource = _activityLog.GetRecentLogs(10);
        }

        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            ActivityTab_Click(sender, e);
        }

        private void RefreshLogButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshLogDisplay();
            AddBotMessage("📋 Activity log refreshed.");
        }

        private void ClearLogButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Clear all activity logs?", "Confirm Clear",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _activityLog.ClearLogs();
                RefreshLogDisplay();
                AddBotMessage("🗑️ Activity log cleared.");
            }
        }

        // ===== TAB SWITCHING =====

        private void TasksTab_Click(object sender, RoutedEventArgs e)
        {
            TasksPanel.Visibility = Visibility.Visible;
            QuizPanel.Visibility = Visibility.Collapsed;
            ActivityPanel.Visibility = Visibility.Collapsed;

            TasksTab.Background = new SolidColorBrush(Color.FromRgb(10, 46, 92));
            QuizTab.Background = new SolidColorBrush(Color.FromRgb(26, 95, 122));
            ActivityTab.Background = new SolidColorBrush(Color.FromRgb(26, 95, 122));

            RefreshTaskList();
        }

        private void QuizTab_Click(object sender, RoutedEventArgs e)
        {
            TasksPanel.Visibility = Visibility.Collapsed;
            QuizPanel.Visibility = Visibility.Visible;
            ActivityPanel.Visibility = Visibility.Collapsed;

            QuizTab.Background = new SolidColorBrush(Color.FromRgb(10, 46, 92));
            TasksTab.Background = new SolidColorBrush(Color.FromRgb(26, 95, 122));
            ActivityTab.Background = new SolidColorBrush(Color.FromRgb(26, 95, 122));
        }

        private void ActivityTab_Click(object sender, RoutedEventArgs e)
        {
            TasksPanel.Visibility = Visibility.Collapsed;
            QuizPanel.Visibility = Visibility.Collapsed;
            ActivityPanel.Visibility = Visibility.Visible;

            ActivityTab.Background = new SolidColorBrush(Color.FromRgb(10, 46, 92));
            TasksTab.Background = new SolidColorBrush(Color.FromRgb(26, 95, 122));
            QuizTab.Background = new SolidColorBrush(Color.FromRgb(26, 95, 122));

            RefreshLogDisplay();
        }
    }
}