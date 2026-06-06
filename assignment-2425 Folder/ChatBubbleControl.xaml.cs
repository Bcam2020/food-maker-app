using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;

namespace assignment_2425.Controls
{
    public partial class ChatBubbleControl : ContentView
    {
        private bool isExpanded = false;

        public ChatBubbleControl()
        {
            InitializeComponent();
            // Initially, translate the control so that a part is off-screen.
            this.TranslationX = 40;
            // Add a swipe gesture recognizer to detect left swipes.
            var swipeGesture = new SwipeGestureRecognizer { Direction = SwipeDirection.Left };
            swipeGesture.Swiped += OnSwiped;
            this.GestureRecognizers.Add(swipeGesture);
        }

        private async void OnSwiped(object sender, SwipedEventArgs e)
        {
            if (!isExpanded)
            {
                // Animate to fully visible.
                await this.TranslateTo(0, 0, 250, Easing.CubicInOut);
                isExpanded = true;
            }
            else
            {
                // Animate back to hidden state.
                await this.TranslateTo(40, 0, 250, Easing.CubicInOut);
                isExpanded = false;
            }
        }

        private async void OnChatIconTapped(object sender, EventArgs e)
        {
            // Open the ChatBotPopup when tapped.
            var chatPopup = new ChatBotPopup();
            await Application.Current.MainPage.ShowPopupAsync(chatPopup);
        }
    }
}
