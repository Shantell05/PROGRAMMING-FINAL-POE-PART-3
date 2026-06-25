using System;
using System.IO;
using System.Media;

namespace CybersecurityChatbotWPF.Services
{
    public class AudioService
    {
        public void PlayVoiceGreeting()
        {
            try
            {
                string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");

                if (!File.Exists(audioPath))
                {
                    audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Audio", "greeting.wav");
                }

                if (File.Exists(audioPath))
                {
                    using (SoundPlayer player = new SoundPlayer(audioPath))
                    {
                        player.PlaySync();
                    }
                }
            }
            catch (Exception)
            {
                // Silently fail
            }
        }
    }
}