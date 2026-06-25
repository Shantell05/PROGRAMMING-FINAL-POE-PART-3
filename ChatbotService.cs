using System.Threading.Tasks;

namespace CybersecurityChatbotWPF.Services
{
    public class ChatbotService
    {
        private readonly EnhancedResponseService _responseService;
        private readonly MemoryManager _memory;
        private readonly SentimentAnalyzer _sentimentAnalyzer;

        public ChatbotService()
        {
            _responseService = new EnhancedResponseService();
            _memory = new MemoryManager();
            _sentimentAnalyzer = new SentimentAnalyzer();
        }

        public MemoryManager Memory => _memory;

        public async Task<string> ProcessUserInput(string userInput)
        {
            await Task.Delay(50);
            _memory.RememberQuestion(userInput);
            string sentiment = _sentimentAnalyzer.DetectSentiment(userInput);
            string response = _responseService.GetResponse(userInput, out string detectedTopic);

            if (!string.IsNullOrEmpty(detectedTopic) && detectedTopic != "fallback" && detectedTopic != "help")
                _memory.RememberTopic(detectedTopic, userInput);

            response = _sentimentAnalyzer.AdjustResponseForSentiment(response, sentiment);

            if (_memory.HasName)
            {
                string prefix = _memory.GetPersonalizedPrefix();
                if (!response.StartsWith(prefix) && !response.Contains(_memory.RecallName()))
                    response = prefix + char.ToLower(response[0]) + response.Substring(1);
            }

            return response;
        }

        public string GetPersonalizedWelcome() => _memory.GetPersonalizedGreeting();
        public void SetUserName(string name) => _memory.RememberName(name);

        public string GetStatistics()
        {
            return $"Session Statistics:\n" +
                   $"- Questions asked: {_memory.UserProfile.QuestionsAsked}\n" +
                   $"- Preferred topic: {(_memory.HasPreferredTopic ? _memory.RecallPreferredTopic() : "Not yet")}";
        }
    }
}