using System;
using System.Collections.Generic;
using System.Linq;
using CybersecurityChatbotWPF.Models;

namespace CybersecurityChatbotWPF.Services
{
    /// <summary>
    /// Manages the cybersecurity quiz
    /// </summary>
    public class QuizManager
    {
        private List<QuizQuestion> _questions;
        private int _currentQuestionIndex;
        private int _score;
        private bool _isQuizActive;
        private List<int> _answeredQuestions;

        public QuizManager()
        {
            _questions = new List<QuizQuestion>();
            _answeredQuestions = new List<int>();
            _currentQuestionIndex = 0;
            _score = 0;
            _isQuizActive = false;
            InitializeQuestions();
        }

        public bool IsQuizActive => _isQuizActive;
        public int TotalQuestions => _questions.Count;
        public int Score => _score;
        public int CurrentQuestionNumber => _currentQuestionIndex + 1;
        public QuizQuestion CurrentQuestion => _isQuizActive && _currentQuestionIndex < _questions.Count
            ? _questions[_currentQuestionIndex]
            : null;

        /// <summary>
        /// Initializes 12 cybersecurity quiz questions
        /// </summary>
        private void InitializeQuestions()
        {
            _questions = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What is the most secure way to create a password?",
                    Options = new List<string> {
                        "Using your pet's name",
                        "Using a combination of uppercase, lowercase, numbers, and symbols",
                        "Using your birthday",
                        "Using 'password123'"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "A strong password uses a mix of character types and is at least 12 characters long.",
                    Category = "Password Safety"
                },
                new QuizQuestion
                {
                    Question = "What should you do if you receive a suspicious email asking for your password?",
                    Options = new List<string> {
                        "Reply with your password",
                        "Forward it to your friends",
                        "Delete it and report it as phishing",
                        "Click all the links to check"
                    },
                    CorrectAnswerIndex = 2,
                    Explanation = "Phishing emails should be reported and deleted. Never share your password via email.",
                    Category = "Phishing"
                },
                new QuizQuestion
                {
                    Question = "What does HTTPS stand for?",
                    Options = new List<string> {
                        "Hyper Text Transfer Protocol Secure",
                        "Hyper Transfer Text Protocol Secure",
                        "High Transfer Text Protocol Secure",
                        "Hyper Text Transfer Process Secure"
                    },
                    CorrectAnswerIndex = 0,
                    Explanation = "HTTPS encrypts data between your browser and websites, keeping your information safe.",
                    Category = "Safe Browsing"
                },
                new QuizQuestion
                {
                    Question = "True or False: You should use the same password for all your accounts.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Using the same password everywhere puts all your accounts at risk if one gets compromised.",
                    Category = "Password Safety"
                },
                new QuizQuestion
                {
                    Question = "What is Two-Factor Authentication (2FA)?",
                    Options = new List<string> {
                        "A type of password",
                        "An extra layer of security beyond your password",
                        "A way to delete accounts",
                        "A type of malware"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "2FA requires a second verification step, like a code from an authenticator app.",
                    Category = "2FA"
                },
                new QuizQuestion
                {
                    Question = "What is a common sign of a phishing email?",
                    Options = new List<string> {
                        "Professional design and perfect grammar",
                        "Urgent language threatening account closure",
                        "A known sender from your contacts",
                        "No attachments"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "Phishing emails often use urgent language to pressure you into acting without thinking.",
                    Category = "Phishing"
                },
                new QuizQuestion
                {
                    Question = "True or False: Free public Wi-Fi is always safe to use.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Public Wi-Fi can be insecure. Always use a VPN on public networks.",
                    Category = "Safe Browsing"
                },
                new QuizQuestion
                {
                    Question = "What is ransomware?",
                    Options = new List<string> {
                        "A type of antivirus",
                        "Malware that encrypts your files and demands payment",
                        "A security update",
                        "A password manager"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "Ransomware locks you out of your files until you pay the attacker.",
                    Category = "Malware"
                },
                new QuizQuestion
                {
                    Question = "What should you do regularly to protect your data?",
                    Options = new List<string> {
                        "Delete all your files",
                        "Backup your important files",
                        "Share passwords with friends",
                        "Turn off your antivirus"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "Regular backups ensure you can recover your data if something goes wrong.",
                    Category = "Data Protection"
                },
                new QuizQuestion
                {
                    Question = "What is social engineering?",
                    Options = new List<string> {
                        "Building social networks",
                        "Manipulating people to reveal confidential information",
                        "Engineer upgrades",
                        "Social media marketing"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "Social engineering tricks people into giving up sensitive information.",
                    Category = "Social Engineering"
                },
                new QuizQuestion
                {
                    Question = "What does a VPN protect you from?",
                    Options = new List<string> {
                        "Only viruses",
                        "Only spam emails",
                        "Eavesdropping on public networks",
                        "Only phishing"
                    },
                    CorrectAnswerIndex = 2,
                    Explanation = "A VPN encrypts your traffic, protecting you from eavesdropping on public Wi-Fi.",
                    Category = "VPN"
                },
                new QuizQuestion
                {
                    Question = "True or False: You should download software from any website that offers it.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Always download software from official websites to avoid malware.",
                    Category = "Malware"
                }
            };
        }

        /// <summary>
        /// Starts the quiz
        /// </summary>
        public void StartQuiz()
        {
            _isQuizActive = true;
            _currentQuestionIndex = 0;
            _score = 0;
            _answeredQuestions.Clear();
            ShuffleQuestions();
        }

        /// <summary>
        /// Shuffles questions for variety
        /// </summary>
        private void ShuffleQuestions()
        {
            Random rng = new Random();
            _questions = _questions.OrderBy(x => rng.Next()).ToList();
        }

        /// <summary>
        /// Submits an answer and returns feedback
        /// </summary>
        public string SubmitAnswer(int selectedIndex)
        {
            if (!_isQuizActive || _currentQuestionIndex >= _questions.Count)
                return "Quiz not active.";

            var question = _questions[_currentQuestionIndex];
            bool isCorrect = selectedIndex == question.CorrectAnswerIndex;

            if (isCorrect)
                _score++;

            _answeredQuestions.Add(_currentQuestionIndex);

            string result = isCorrect ? "✅ Correct! " : "❌ Incorrect. ";
            result += question.Explanation;

            _currentQuestionIndex++;

            // Check if quiz is complete
            if (_currentQuestionIndex >= _questions.Count)
            {
                _isQuizActive = false;
                result += $"\n\n🏆 Quiz Complete! You scored {_score}/{_questions.Count}.";
                result += GetScoreFeedback();
            }

            return result;
        }

        /// <summary>
        /// Gets feedback based on score
        /// </summary>
        private string GetScoreFeedback()
        {
            double percentage = (double)_score / _questions.Count * 100;

            if (percentage >= 90)
                return " 🌟 Excellent! You're a cybersecurity expert!";
            else if (percentage >= 70)
                return " 👍 Good job! Keep learning to become a cybersecurity pro!";
            else if (percentage >= 50)
                return " 📚 Not bad! Review the topics you missed to improve!";
            else
                return " 💪 Keep learning! Cybersecurity is important for everyone!";
        }

        /// <summary>
        /// Gets quiz progress
        /// </summary>
        public string GetProgress()
        {
            if (!_isQuizActive && _currentQuestionIndex >= _questions.Count)
                return $"Quiz completed! Score: {_score}/{_questions.Count}";

            return $"Question {_currentQuestionIndex + 1}/{_questions.Count} | Score: {_score}";
        }
    }
}