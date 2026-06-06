using assignment_2425.Models;
using assignment_2425.Services;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Devices;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace assignment_2425
{
    [QueryProperty(nameof(FriendId), "friendId")]
    public partial class FriendRecipesPage : ContentPage
    {
        public ObservableCollection<Recipe> FriendRecipes { get; set; } = new ObservableCollection<Recipe>();
        public string FriendId { get; set; }
        public FriendRecipesPage()
        {
            InitializeComponent();
            BindingContext = this;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            App.SetAppTheme();
            await LoadFriendRecipesAsync();
        }
        private async Task LoadFriendRecipesAsync()
        {
            try
            {
                string authToken = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(authToken))
                {
                    await DisplayAlert("Error", "User not authenticated.", "OK");
                    return;
                }
                var recipes = await FirestoreService.GetUserRecipesAsync(FriendId, authToken);
                FriendRecipes.Clear();
                foreach (var recipe in recipes)
                {
                    FriendRecipes.Add(recipe);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load friend's recipes: {ex.Message}", "OK");
            }
        }
        private async void OnRefresh(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            await LoadFriendRecipesAsync();
            RecipesRefreshView.IsRefreshing = false;
        }
        private async void OnBackClicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            await Shell.Current.GoToAsync("FriendsListPage");
        }
    }
}
