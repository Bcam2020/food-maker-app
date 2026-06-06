using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using assignment_2425.Models;
using assignment_2425.Services;
using System.Diagnostics;

namespace assignment_2425.ViewModels
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        private readonly ChatService _chatService;

        public ObservableCollection<ChatMessage> ChatMessages { get; } = new ObservableCollection<ChatMessage>();

        private string _currentInput;
        public string CurrentInput
        {
            get => _currentInput;
            set { _currentInput = value; OnPropertyChanged(); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        // Additional UI context (can be updated externally).
        public string PageContext { get; set; } = "on the home page";

        public ICommand SendCommand { get; }

        public ChatViewModel()
        {
            _chatService = new ChatService();
            SendCommand = new Command(async () => await SendMessageAsync(), () => !IsBusy);
        }

        private async Task SendMessageAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentInput))
                return;

            try
            {
                IsBusy = true;
                ((Command)SendCommand).ChangeCanExecute();

                string userMsg = CurrentInput;
                AddUserMessage(userMsg);

                // Simulate a brief delay to mimic processing.
                await Task.Delay(500);
                string botReply = _chatService.GetResponse(userMsg);
                AddBotMessage(botReply);

                // Trigger navigation if the response includes keywords.
                if (botReply.ToLower().Contains("profile"))
                    MessagingCenter.Send(this, "NavigateToProfile");
                else if (botReply.ToLower().Contains("settings"))
                    MessagingCenter.Send(this, "NavigateToSettings");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing message: {ex.Message}");
                AddBotMessage("Bot: Sorry, something went wrong.");
            }
            finally
            {
                CurrentInput = string.Empty;
                OnPropertyChanged(nameof(CurrentInput));
                IsBusy = false;
                ((Command)SendCommand).ChangeCanExecute();
            }
        }

        private void AddUserMessage(string message)
        {
            ChatMessages.Add(new ChatMessage
            {
                Sender = "You",
                Content = message,
                Timestamp = DateTime.Now
            });
        }

        private void AddBotMessage(string message)
        {
            ChatMessages.Add(new ChatMessage
            {
                Sender = "Bot",
                Content = message,
                Timestamp = DateTime.Now
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
