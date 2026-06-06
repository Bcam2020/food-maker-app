using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using CommunityToolkit.Maui.Views;
using System;
using System.Threading.Tasks;

namespace assignment_2425
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        // Animate and navigate to IngredientsPage
        private async void OnAddIngredientsClicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            if (sender is VisualElement element)
            {
                await element.ScaleTo(1.05, 100);
                await Task.Delay(50);
                await element.ScaleTo(1, 100);
            }
            await Shell.Current.GoToAsync(nameof(IngredientsPage));
        }

        // Navigate to SettingsPage
        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            await Shell.Current.GoToAsync(nameof(SettingsPage));
        }

        // Navigate to RecipePage
        private async void OnRecipesClicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            await Shell.Current.GoToAsync(nameof(RecipePage));
        }

        // Navigate to LoginPage (for profile access)
        private async void OnProfileClicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }

        // Show the chat bot popup
        private async void OnChatBotClicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            var chatPopup = new ChatBotPopup();
            await this.ShowPopupAsync(chatPopup);
        }
    }
}
