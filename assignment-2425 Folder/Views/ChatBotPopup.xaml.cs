using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;

namespace assignment_2425
{
    // Base class for ViewModels implementing INotifyPropertyChanged
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;
            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Model for a chat message
    public class ChatMessage
    {
        public string Content { get; set; }
        public string Sender { get; set; }
        public DateTime Timestamp { get; set; }
    }

    // ViewModel for the chat functionality
    public class ChatViewModel : BaseViewModel
    {
        public ObservableCollection<ChatMessage> ChatMessages { get; set; } = new ObservableCollection<ChatMessage>();

        string currentInput;
        public string CurrentInput
        {
            get => currentInput;
            set => SetProperty(ref currentInput, value);
        }

        public ICommand SendCommand { get; }

        public ChatViewModel()
        {
            SendCommand = new Command(SendMessage);
        }

        // Sends a user message and gets a bot response
        void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(CurrentInput))
                return;
            ChatMessages.Add(new ChatMessage { Content = CurrentInput, Sender = "User", Timestamp = DateTime.Now });
            string response = GetBotResponse(CurrentInput);
            ChatMessages.Add(new ChatMessage { Content = response, Sender = "Bot", Timestamp = DateTime.Now });
            CurrentInput = string.Empty;
        }

        // Determine bot response based on user input using regex checks
        string GetBotResponse(string input)
        {
            string lower = input.ToLower();
            if (Regex.IsMatch(lower, @"\b(community|forum)\b"))
                return "Go to the Community tab to access the community page.";
            if (Regex.IsMatch(lower, @"\b(recipe|recipes|cook|food)\b"))
                return "Tap the pan icon to find recipes.";
            if (Regex.IsMatch(lower, @"\b(profile)\b"))
            {
                MessagingCenter.Send(this, "NavigateToProfile");
                return "Navigating to your profile.";
            }
            if (Regex.IsMatch(lower, @"\b(setting|options|preferences)\b"))
            {
                MessagingCenter.Send(this, "NavigateToSettings");
                return "Navigating to settings.";
            }
            return "I didn't understand. Try asking about community, recipes, profile, or settings.";
        }
    }

    // Popup for the chat bot, using CommunityToolkit.Maui.Views.Popup
    public partial class ChatBotPopup : Popup
    {
        public ChatViewModel ViewModel { get; set; }

        public ChatBotPopup()
        {
            InitializeComponent();
            ViewModel = new ChatViewModel();
            BindingContext = ViewModel;
            Opened += ChatBotPopup_Opened;
            Closed += ChatBotPopup_Closed;
        }

        // Subscribe to navigation messages when popup opens
        void ChatBotPopup_Opened(object sender, PopupOpenedEventArgs e)
        {
            MessagingCenter.Subscribe<ChatViewModel>(this, "NavigateToProfile", async (senderVm) =>
            {
                bool navigate = await Application.Current.MainPage.DisplayAlert("Navigation", "Would you like to go to your profile now?", "Yes", "No");
                if (navigate)
                {
                    Close();
                    await Shell.Current.GoToAsync(nameof(ProfilePage));
                }
            });
            MessagingCenter.Subscribe<ChatViewModel>(this, "NavigateToSettings", async (senderVm) =>
            {
                bool navigate = await Application.Current.MainPage.DisplayAlert("Navigation", "Would you like to go to the settings page now?", "Yes", "No");
                if (navigate)
                {
                    Close();
                    await Shell.Current.GoToAsync(nameof(SettingsPage));
                }
            });
        }

        // Unsubscribe from messages when popup closes
        void ChatBotPopup_Closed(object sender, PopupClosedEventArgs e)
        {
            MessagingCenter.Unsubscribe<ChatViewModel>(this, "NavigateToProfile");
            MessagingCenter.Unsubscribe<ChatViewModel>(this, "NavigateToSettings");
        }
    }
}
