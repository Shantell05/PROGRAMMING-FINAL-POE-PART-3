using CybersecurityChatbotWPF.Models;

namespace CybersecurityChatbotWPF.Services
{
    public class MemoryManager
    {
        private UserProfile _userProfile;

        public MemoryManager()
        {
            _userProfile = new UserProfile();
        }

        public UserProfile UserProfile => _userProfile;

        public void RememberName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name) && name.Length >= 2)
            {
                _userProfile.Name = name;
            }
        }

        public void RememberTopic(string topic, string detail)
        {
            _userProfile.PreferredTopic = topic;
            _userProfile.LastQuestionTopic = topic;
        }

        public void RememberQuestion(string question)
        {
            _userProfile.QuestionsAsked++;
        }

        public string RecallName()
        {
            return _userProfile.Name;
        }

        public string RecallPreferredTopic()
        {
            return _userProfile.PreferredTopic;
        }

        public string GetPersonalizedGreeting()
        {
            if (!string.IsNullOrEmpty(_userProfile.Name))
            {
                if (!string.IsNullOrEmpty(_userProfile.PreferredTopic))
                {
                    return $"Welcome back, {_userProfile.Name}! As someone interested in {_userProfile.PreferredTopic}, I have more tips for you!";
                }
                return $"Welcome back, {_userProfile.Name}! Ready to learn more about cybersecurity?";
            }
            return "Welcome! I'm your Cybersecurity Awareness Bot!";
        }

        public string GetPersonalizedPrefix()
        {
            if (!string.IsNullOrEmpty(_userProfile.Name))
            {
                return $"{_userProfile.Name}, ";
            }
            return "";
        }

        public bool HasName => !string.IsNullOrEmpty(_userProfile.Name);
        public bool HasPreferredTopic => !string.IsNullOrEmpty(_userProfile.PreferredTopic);
        public string LastTopic => _userProfile.LastQuestionTopic;
    }
}