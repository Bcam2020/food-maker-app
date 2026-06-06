using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using assignment_2425.Models;
using assignment_2425.Services;

namespace assignment_2425
{
    public partial class FriendsListPage : ContentPage
    {
        public ObservableCollection<Friend> Friends { get; set; } = new ObservableCollection<Friend>();

        public FriendsListPage()
        {
            InitializeComponent();
            BindingContext = this;
            // Load the friends on page initialization.
            LoadFriendsAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Subscribe to notifications for friend additions (if needed)
            MessagingCenter.Subscribe<SearchUsersPage>(this, "FriendAdded", async (sender) =>
            {
                await LoadFriendsAsync();
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<SearchUsersPage>(this, "FriendAdded");
        }

        private async Task LoadFriendsAsync()
        {
            try
            {
                string userId = await SecureStorage.GetAsync("firebase_user_id");
                string authToken = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(authToken))
                {
                    await DisplayAlert("Error", "User not authenticated.", "OK");
                    return;
                }
                var friendsList = await FirestoreService.GetUserFriendsAsync(userId, authToken);
                Friends.Clear();
                foreach (var friend in friendsList)
                {
                    Friends.Add(friend);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to find recipes: response status code does not indicate success: {ex.Message}", "OK");
            }
        }

        private async void OnViewFriendRecipesClicked(object sender, EventArgs e)
        {
            // Retrieve the FriendId from the button's CommandParameter.
            string friendId = (sender as Button)?.CommandParameter as string;
            if (string.IsNullOrEmpty(friendId))
            {
                await DisplayAlert("Error", "Friend not selected.", "OK");
                return;
            }
            // Navigate to FriendRecipesPage with the friendId as a query parameter.
            await Shell.Current.GoToAsync($"{nameof(FriendRecipesPage)}?friendId={friendId}");
        }

        private async void OnDeleteFriendClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Friend friend)
            {
                bool confirm = await DisplayAlert("Confirm Delete", $"Remove \"{friend.Name}\" from friends?", "Yes", "No");
                if (confirm)
                {
                    string userId = await SecureStorage.GetAsync("firebase_user_id");
                    string authToken = await SecureStorage.GetAsync("auth_token");
                    try
                    {
                        await FirestoreService.DeleteFriendAsync(userId, friend.FriendId, authToken);
                        // Instead of only removing the friend from the collection,
                        // reload the list to ensure the UI reflects the change.
                        await LoadFriendsAsync();
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"Could not remove friend: {ex.Message}", "OK");
                    }
                }
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///ProfilePage");
        }

        private async void OnSearchClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("/SearchUsersPage");
        }

        protected override bool OnBackButtonPressed()
        {
            Shell.Current.GoToAsync("///ProfilePage");
            return true;
        }
    }
}
