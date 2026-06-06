using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using FuzzySharp;
using System.Collections.Generic;
using assignment_2425.Models;

namespace assignment_2425.ViewModels
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ChatMessage> ChatMessages { get; set; } = new ObservableCollection<ChatMessage>();

        // This property can be set externally (for example, from the MainPage) to inform the bot of the current UI state.
        public string PageContext { get; set; } = "on the home page";

        private string _currentInput;
        public string CurrentInput
        {
            get => _currentInput;
            set
            {
                _currentInput = value;
                OnPropertyChanged();
            }
        }

        // A dictionary of known intents (keywords) mapped to their responses.
        private static readonly Dictionary<string, string> BotIntents = new Dictionary<string, string>
        {
            { "profile", "You can access your profile by tapping the 'Profile' tab at the bottom." },
            { "settings", "You can access settings by tapping the toolbar at the top." },
            { "ingredient", "To add ingredients, tap the ingredient button on the home screen." },
            { "recipe", "Browse recipes by tapping the recipe icon." },
            { "help", "I'm here to help! Try asking about profile, settings, ingredients, or recipes." }
        };

        public ICommand SendCommand { get; }

        public ChatViewModel()
        {
            SendCommand = new Command(async () => await SendMessageAsync());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        /// <summary>
        /// Adds a user message to the conversation.
        /// </summary>
        public void AddUserMessage(string message)
        {
            ChatMessages.Add(new ChatMessage { Sender = "You", Content = message, Timestamp = DateTime.Now });
        }

        /// <summary>
        /// Adds a bot message to the conversation.
        /// </summary>
        public void AddBotMessage(string message)
        {
            ChatMessages.Add(new ChatMessage { Sender = "Bot", Content = message, Timestamp = DateTime.Now });
        }

        /// <summary>
        /// Uses fuzzy matching to determine the best intent response.
        /// </summary>
        public string GetFuzzyResponse(string userMessage)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
                return "Bot: Please enter a message.";

            string lowerMessage = userMessage.ToLower();
            var bestMatch = (intent: (string)null, score: 0);

            foreach (var kvp in BotIntents)
            {
                int score = Fuzz.PartialRatio(lowerMessage, kvp.Key.ToLower());
                if (score > bestMatch.score)
                    bestMatch = (kvp.Key, score);
            }

            if (bestMatch.score >= 60)
                return $"Bot: {BotIntents[bestMatch.intent]}";
            else
                return "Bot: I'm not sure I understand. Could you please rephrase?";
        }

        /// <summary>
        /// Processes the user's message and returns a response using fuzzy matching.
        /// </summary>
        public string ProcessMessage(string userMessage)
        {
            // You might also incorporate PageContext here for more context-aware responses.
            return GetFuzzyResponse(userMessage);
        }

        /// <summary>
        /// Command method that sends the user's message, processes it, and raises navigation events when applicable.
        /// </summary>
        private async Task SendMessageAsync()
        {
            if (!string.IsNullOrWhiteSpace(CurrentInput))
            {
                string userMsg = CurrentInput;
                AddUserMessage(userMsg);
                string botReply = ProcessMessage(userMsg);
                AddBotMessage(botReply);
                CurrentInput = string.Empty;
                OnPropertyChanged(nameof(CurrentInput));

                // Check for navigation triggers in the response.
                if (botReply.ToLower().Contains("profile"))
                {
                    // Notify subscribers that navigation to profile is requested.
                    MessagingCenter.Send(this, "NavigateToProfile");
                }
                else if (botReply.ToLower().Contains("settings"))
                {
                    MessagingCenter.Send(this, "NavigateToSettings");
                }
                await Task.CompletedTask;
            }
        }
    }
}
