using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CybersecurityChatbotWPF.Services
{
    /// <summary>
    /// Simulates Natural Language Processing using keyword detection
    /// </summary>
    public class NLPService
    {
        private Dictionary<string, string> _intentPatterns;
        private Dictionary<string, List<string>> _synonyms;

        public NLPService()
        {
            _intentPatterns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _synonyms = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            InitializePatterns();
            InitializeSynonyms();
        }

        private void InitializePatterns()
        {
            _intentPatterns["add_task"] = @"(?:add|create|new|make|set up|setup)\s*(?:a\s*)?(?:task|reminder|todo)";
            _intentPatterns["show_tasks"] = @"(?:show|view|display|list|get)\s*(?:my\s*)?(?:tasks|task|todo|reminders)";
            _intentPatterns["complete_task"] = @"(?:complete|finish|done|mark)\s*(?:task|todo|reminder)";
            _intentPatterns["delete_task"] = @"(?:delete|remove|cancel|clear)\s*(?:task|todo|reminder)";
            _intentPatterns["start_quiz"] = @"(?:start|take|do|begin|play)\s*(?:a\s*)?(?:quiz|test|game)";
            _intentPatterns["show_activity"] = @"(?:show|view|display|what)\s*(?:activity|log|history|actions|done|me)";
            _intentPatterns["help"] = @"(?:help|menu|options|what can|assist)";
        }

        private void InitializeSynonyms()
        {
            _synonyms["task"] = new List<string> { "todo", "reminder", "remind", "note", "action" };
            _synonyms["add"] = new List<string> { "create", "new", "make", "set", "setup" };
            _synonyms["show"] = new List<string> { "view", "display", "list", "get", "tell" };
            _synonyms["complete"] = new List<string> { "finish", "done", "mark", "check" };
            _synonyms["delete"] = new List<string> { "remove", "cancel", "clear", "erase" };
            _synonyms["quiz"] = new List<string> { "test", "game", "question", "trivia" };
            _synonyms["activity"] = new List<string> { "log", "history", "actions", "done" };
        }

        /// <summary>
        /// Detects intent from user input
        /// </summary>
        public string DetectIntent(string userInput)
        {
            string input = userInput.ToLower().Trim();

            foreach (var pattern in _intentPatterns)
            {
                if (Regex.IsMatch(input, pattern.Value, RegexOptions.IgnoreCase))
                {
                    return pattern.Key;
                }
            }

            return "unknown";
        }

        /// <summary>
        /// Extracts key information from user input
        /// </summary>
        public string ExtractTaskDetails(string userInput)
        {
            // Remove common words to extract the task description
            string[] removeWords = { "add", "create", "new", "make", "set", "setup", "a", "task", "reminder", "todo" };
            string result = userInput;

            foreach (string word in removeWords)
            {
                result = Regex.Replace(result, $@"\b{word}\b", "", RegexOptions.IgnoreCase);
            }

            result = Regex.Replace(result, @"\s+", " ").Trim();

            // Clean up multiple spaces
            return string.IsNullOrEmpty(result) ? "Cybersecurity task" : result;
        }

        /// <summary>
        /// Extracts reminder details from user input
        /// </summary>
        public (bool hasReminder, int days) ExtractReminderInfo(string userInput)
        {
            string input = userInput.ToLower();

            // Check for "remind me in X days"
            var match = Regex.Match(input, @"remind\s+me\s+in\s+(\d+)\s+(day|days|hour|hours)");
            if (match.Success)
            {
                int number = int.Parse(match.Groups[1].Value);
                string unit = match.Groups[2].Value;

                if (unit.StartsWith("hour"))
                    return (true, number / 24); // Convert hours to days (approx)
                else
                    return (true, number);
            }

            // Check for "tomorrow"
            if (input.Contains("tomorrow"))
                return (true, 1);

            // Check for "next week"
            if (input.Contains("next week"))
                return (true, 7);

            // Check for "in a week"
            if (input.Contains("in a week"))
                return (true, 7);

            // Check for "in X days" (variation)
            match = Regex.Match(input, @"in\s+(\d+)\s+(day|days)");
            if (match.Success)
            {
                return (true, int.Parse(match.Groups[1].Value));
            }

            return (false, 0);
        }

        /// <summary>
        /// Checks if input is a follow-up request
        /// </summary>
        public bool IsFollowUpRequest(string userInput)
        {
            string[] followUpPhrases = {
                "tell me more", "more info", "continue", "elaborate",
                "another", "next", "and then", "go on", "more details"
            };

            foreach (var phrase in followUpPhrases)
            {
                if (userInput.ToLower().Contains(phrase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the appropriate response based on intent
        /// </summary>
        public string GetNLPResponse(string intent, string userInput, out bool isHandled)
        {
            isHandled = false;

            switch (intent)
            {
                case "add_task":
                    isHandled = true;
                    string taskDetails = ExtractTaskDetails(userInput);
                    return $"I understand you want to add a task: '{taskDetails}'. Would you like to set a reminder for this task? (Say 'yes' or 'no')";

                case "show_tasks":
                    isHandled = true;
                    return "Showing your tasks... Let me retrieve them for you.";

                case "complete_task":
                    isHandled = true;
                    return "Which task would you like to mark as completed? Please tell me the task title.";

                case "delete_task":
                    isHandled = true;
                    return "Which task would you like to delete? Please tell me the task title.";

                case "start_quiz":
                    isHandled = true;
                    return "Starting the cybersecurity quiz! Get ready to test your knowledge!";

                case "show_activity":
                    isHandled = true;
                    return "Here's your activity log. Let me retrieve it for you.";

                case "help":
                    isHandled = true;
                    return "I can help you with:\n- Adding tasks (say 'add task')\n- Viewing tasks\n- Starting the cybersecurity quiz\n- Showing your activity log\n- Getting cybersecurity tips";

                default:
                    return "";
            }
        }
    }
}