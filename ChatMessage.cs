using System;

namespace assignment_2425.Models
{
    /// <summary>
    /// Represents a single message in the chat conversation.
    /// </summary>
    public class ChatMessage
    {
        public string Sender { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
