using System.Collections.Generic;

namespace CybersecurityChatbotWPF.Models
{
    public class UserProfile
    {
        public string Name { get; set; } = string.Empty;
        public string PreferredTopic { get; set; } = string.Empty;
        public string LastQuestionTopic { get; set; } = string.Empty;
        public int QuestionsAsked { get; set; }
        public List<string> ConversationHistory { get; set; } = new List<string>();
        public Dictionary<string, string> UserInterests { get; set; } = new Dictionary<string, string>();
    }
}