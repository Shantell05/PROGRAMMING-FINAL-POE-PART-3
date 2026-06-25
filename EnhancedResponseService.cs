using System;
using System.Collections.Generic;

namespace CybersecurityChatbotWPF.Services
{
    public class EnhancedResponseService
    {
        public int TotalResponsesGiven { get; private set; }
        private Dictionary<string, List<string>> _keywordResponses;
        private Dictionary<string, List<string>> _randomResponses;
        private List<string> _fallbackResponses;
        private Random _random;

        public EnhancedResponseService()
        {
            TotalResponsesGiven = 0;
            _random = new Random();
            _keywordResponses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            _randomResponses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            _fallbackResponses = new List<string>();
            InitializeResponses();
            InitializeRandomResponses();
            InitializeFallbackResponses();
        }

        private void InitializeResponses()
        {
            _keywordResponses["password"] = new List<string> {
                "Use 12+ characters with uppercase, lowercase, numbers, and symbols!",
                "Never reuse passwords across accounts!",
                "Use a password manager like Bitwarden!",
                "Enable 2FA on all accounts!"
            };
            _keywordResponses["phishing"] = new List<string> {
                "Check sender's email address carefully!",
                "Never click suspicious links!",
                "Look for spelling errors!",
                "Report phishing emails!"
            };
            _keywordResponses["privacy"] = new List<string> {
                "Review privacy settings regularly!",
                "Use encrypted messaging apps!",
                "Be careful what you share online!",
                "Use a VPN on public Wi-Fi!"
            };
            _keywordResponses["2fa"] = new List<string> {
                "Two-Factor Authentication adds extra security!",
                "Use authenticator apps, not SMS!",
                "Enable 2FA on all accounts!",
                "Store backup codes safely!"
            };
            _keywordResponses["malware"] = new List<string> {
                "Install antivirus software!",
                "Keep everything updated!",
                "Don't download from untrusted sources!",
                "Run regular system scans!"
            };
            _keywordResponses["vpn"] = new List<string> {
                "A VPN encrypts your internet traffic!",
                "Use a VPN on public Wi-Fi!",
                "Choose a paid VPN without logs!",
                "Free VPNs often sell your data!"
            };
        }

        private void InitializeRandomResponses()
        {
            _randomResponses["phishing tip"] = new List<string> {
                "Be cautious of emails asking for personal information.",
                "Check the sender's email address carefully.",
                "Never click links in suspicious emails.",
                "If it creates urgency, it's likely phishing."
            };
            _randomResponses["password tip"] = new List<string> {
                "Use a passphrase: combine 4 random words.",
                "Never use personal information in passwords.",
                "Change passwords after a data breach.",
                "Use different passwords for each account."
            };
            _randomResponses["tip"] = new List<string> {
                "Keep your software updated regularly.",
                "Backup your important files.",
                "Use a password manager.",
                "Enable automatic updates.",
                "Be skeptical of unsolicited messages."
            };
        }

        private void InitializeFallbackResponses()
        {
            _fallbackResponses = new List<string> {
                "I'm not sure I understand. Could you rephrase?",
                "Try asking about passwords, phishing, privacy, or 2FA!",
                "Type 'help' to see all topics I can help with!",
                "I specialise in cybersecurity - ask me anything!"
            };
        }

        private bool IsFollowUpRequest(string input)
        {
            string[] phrases = { "another tip", "tell me more", "explain more", "more info", "continue", "elaborate" };
            foreach (var phrase in phrases)
                if (input.Contains(phrase)) return true;
            return false;
        }

        public string GetResponse(string userInput, out string detectedTopic)
        {
            TotalResponsesGiven++;
            string lowerInput = userInput.Trim().ToLower();
            detectedTopic = "";

            if (IsFollowUpRequest(lowerInput))
            {
                detectedTopic = "followup";
                string[] followUpResponses = { "Here's another tip: ", "Sure! Let me share more: ", "Of course! Here's more info: " };
                return followUpResponses[_random.Next(followUpResponses.Length)] + GetRandomResponse("tip");
            }

            foreach (var keyword in _keywordResponses.Keys)
            {
                if (lowerInput.Contains(keyword.ToLower()))
                {
                    detectedTopic = keyword;
                    int index = _random.Next(_keywordResponses[keyword].Count);
                    return _keywordResponses[keyword][index];
                }
            }

            foreach (var randomKey in _randomResponses.Keys)
            {
                if (lowerInput.Contains(randomKey))
                {
                    detectedTopic = randomKey;
                    return GetRandomResponse(randomKey);
                }
            }

            if (lowerInput.Contains("help") || lowerInput.Contains("menu"))
            {
                detectedTopic = "help";
                return "I can help with: password safety, phishing, privacy, 2FA, malware, VPN, and more!";
            }

            detectedTopic = "fallback";
            return _fallbackResponses[_random.Next(_fallbackResponses.Count)];
        }

        private string GetRandomResponse(string key)
        {
            if (_randomResponses.ContainsKey(key) && _randomResponses[key].Count > 0)
            {
                int index = _random.Next(_randomResponses[key].Count);
                return _randomResponses[key][index];
            }
            if (_randomResponses.ContainsKey("tip") && _randomResponses["tip"].Count > 0)
            {
                int index = _random.Next(_randomResponses["tip"].Count);
                return _randomResponses["tip"][index];
            }
            return "Here's a cybersecurity tip: stay safe online!";
        }
    }
}