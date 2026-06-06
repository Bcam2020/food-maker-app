using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using assignment_2425.Models;
using assignment_2425.Services;

namespace assignment_2425
{
    // Helper class to present search results.
    public class UserSearchResult
    {
        public string UserId { get; set; }
        public string UsernameOrEmail { get; set; }
    }

    public partial class SearchUsersPage : ContentPage
    {
        public ObservableCollection<UserSearchResult> SearchResults { get; set; } = new ObservableCollection<UserSearchResult>();

        public SearchUsersPage()
        {
            InitializeComponent();
            BindingContext = this;
            UsersCollectionView.ItemsSource = SearchResults;
        }

        // Called when the search button is pressed on the SearchBar.
        private async void OnSearchBarButtonPressed(object sender, EventArgs e)
        {
            string query = UserSearchBar.Text?.Trim() ?? "";
            await PerformSearchAsync(query);
        }

        // Called when the text changes in the SearchBar.
        private async void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            await PerformSearchAsync(e.NewTextValue);
        }

        // Searches Firestore for all users, then filters by username or email.
        private async Task PerformSearchAsync(string searchTerm)
        {
            try
            {
                string authToken = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(authToken))
                {
                    await DisplayAlert("Error", "You must be logged in.", "OK");
                    return;
                }

                // Clear previous results.
                SearchResults.Clear();

                // Retrieve all user profiles from Firestore.
                var allUsers = await FirestoreService.SearchAllUsersAsync(authToken);

                // For debugging, you might log the count:
                System.Diagnostics.Debug.WriteLine($"Total users returned: {allUsers.Count}");

                // Filter users based on searchTerm.
                var filtered = allUsers.Where(u =>
                    u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .Select(u => new UserSearchResult
                    {
                        UserId = u.UserId,
                        UsernameOrEmail = $"{u.Username} ({u.Email})"
                    }).ToList();

                // If searchTerm is empty, optionally display all users.
                // For example:
                // if (string.IsNullOrEmpty(searchTerm))
                //     filtered = allUsers.Select(u => new UserSearchResult { UserId = u.UserId, UsernameOrEmail = $"{u.Username} ({u.Email})" }).ToList();

                foreach (var user in filtered)
                {
                    SearchResults.Add(user);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Search failed: {ex.Message}", "OK");
            }
        }

        private async void OnAddFriendClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var userResult = button?.CommandParameter as UserSearchResult;
            if (userResult == null)
            {
                await DisplayAlert("Error", "Friend not selected.", "OK");
                return;
            }
            try
            {
                string localUserId = await SecureStorage.GetAsync("firebase_user_id");
                string authToken = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(localUserId) || string.IsNullOrEmpty(authToken))
                {
                    await DisplayAlert("Error", "You must be logged in to add friends.", "OK");
                    return;
                }

                // Check if friend already exists.
                var currentFriends = await FirestoreService.GetUserFriendsAsync(localUserId, authToken);
                if (currentFriends.Any(f => f.FriendId == userResult.UserId))
                {
                    await DisplayAlert("Already Added", "This friend is already in your friend list.", "OK");
                    return;
                }

                await FirestoreService.AddFriendAsync(localUserId, userResult.UserId, userResult.UsernameOrEmail, authToken);
                await DisplayAlert("Success", $"Friend '{userResult.UsernameOrEmail}' added!", "OK");

                // Notify friend list to refresh.
                MessagingCenter.Send(this, "FriendAdded");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not add friend: {ex.Message}", "OK");
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("/FriendsListPage");
        }

        protected override bool OnBackButtonPressed()
        {
            Shell.Current.GoToAsync("/FriendsListPage");
            return true;
        }
    }
}
