using System;
using System.Windows;
using System.Windows.Media;

namespace CybersecurityChatbotWPF.Models
{
    public class ChatMessage
    {
        public string Text { get; set; } = string.Empty;
        public bool IsUser { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public System.Windows.HorizontalAlignment Alignment { get; set; } = System.Windows.HorizontalAlignment.Left;
        public System.Windows.Media.Brush BubbleColor { get; set; } = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);

        public string FormattedTime => Timestamp.ToString("hh:mm tt");
    }
}