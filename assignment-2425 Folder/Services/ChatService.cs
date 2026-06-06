using System;
using System.Collections.Generic;
using FuzzySharp;

namespace assignment_2425.Services
{
    public class ChatService
    {
        private static readonly Dictionary<string, string> BotIntents = new Dictionary<string, string>
        {
            { "profile", "You can access your profile by tapping the 'Profile' tab at the bottom." },
            { "settings", "You can access settings by tapping the toolbar at the top." },
            { "ingredient", "To add ingredients, tap the ingredient button on the home screen." },
            { "recipe", "Browse recipes by tapping the recipe icon." },
            { "help", "I'm here to help! Try asking about profile, settings, ingredients, or recipes." }
        };

        public string GetResponse(string userMessage)
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
    }
}
