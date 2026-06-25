using System;
using System.Collections.Generic;

namespace CybersecurityChatbotWPF.Services
{
    public class SentimentAnalyzer
    {
        private Dictionary<string, string> _sentimentKeywords;
        private Dictionary<string, string> _sentimentResponses;

        public SentimentAnalyzer()
        {
            _sentimentKeywords = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _sentimentResponses = new Dictionary<string, string>();
            InitializeKeywords();
            InitializeResponses();
        }

        private void InitializeKeywords()
        {
            _sentimentKeywords["worried"] = "worried";
            _sentimentKeywords["anxious"] = "worried";
            _sentimentKeywords["nervous"] = "worried";
            _sentimentKeywords["scared"] = "worried";
            _sentimentKeywords["frustrated"] = "frustrated";
            _sentimentKeywords["confused"] = "frustrated";
            _sentimentKeywords["curious"] = "curious";
            _sentimentKeywords["interested"] = "curious";
            _sentimentKeywords["great"] = "positive";
            _sentimentKeywords["good"] = "positive";
            _sentimentKeywords["thanks"] = "positive";
            _sentimentKeywords["helpful"] = "positive";
        }

        private void InitializeResponses()
        {
            _sentimentResponses["worried"] = "It's completely understandable to feel that way. Cybersecurity concerns are valid. Let me help you stay safe.";
            _sentimentResponses["frustrated"] = "I understand this can be frustrating. Let's take it step by step together. Here's some help:";
            _sentimentResponses["curious"] = "That's fantastic that you're curious about cybersecurity! Learning is the first step to staying safe. Here's what you should know:";
            _sentimentResponses["positive"] = "I'm really glad you're finding this helpful! Your positive attitude makes learning cybersecurity more effective!";
        }

        public string DetectSentiment(string userInput)
        {
            string lowerInput = userInput.ToLower();
            foreach (var keyword in _sentimentKeywords)
            {
                if (lowerInput.Contains(keyword.Key))
                    return keyword.Value;
            }
            return "neutral";
        }

        public string AdjustResponseForSentiment(string response, string sentiment)
        {
            if (sentiment == "neutral") return response;
            if (_sentimentResponses.ContainsKey(sentiment))
                return _sentimentResponses[sentiment] + "\n\n" + response;
            return response;
        }
    }
}