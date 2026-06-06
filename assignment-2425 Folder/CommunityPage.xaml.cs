using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using assignment_2425.Models;
using assignment_2425.Services;

namespace assignment_2425
{
    public partial class CommunityPage : ContentPage
    {
        // List of community recipes
        public ObservableCollection<CommunityRecipe> CommunityRecipes { get; set; } = new ObservableCollection<CommunityRecipe>();
        public bool IsRefreshing { get; set; } = false;
        public Command RefreshCommand { get; }
        private bool isPollingActive = false;

        public CommunityPage()
        {
            InitializeComponent();
            BindingContext = this;
            RefreshCommand = new Command(async () => await RefreshCommunityAsync());
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            App.SetAppTheme();
            await LoadCommunityRecipesAsync();
            if (!isPollingActive)
            {
                isPollingActive = true;
                Device.StartTimer(TimeSpan.FromSeconds(30), () =>
                {
                    RefreshCommunityAsync();
                    return isPollingActive;
                });
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            isPollingActive = false;
        }

        // Load community recipes from Firestore
        private async Task LoadCommunityRecipesAsync()
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            CommunityRecipes.Clear();
            string authToken = await SecureStorage.GetAsync("auth_token");
            try
            {
                var users = await FirestoreService.SearchAllUsersAsync(authToken);
                foreach (var user in users)
                {
                    var recipes = await FirestoreService.GetUserRecipesAsync(user.UserId, authToken);
                    foreach (var recipe in recipes)
                    {
                        string profilePicture = await GetUserProfilePicture(user.UserId, authToken);
                        var recentComments = await FirestoreService.GetCommentsForRecipeAsync(recipe.RecipeId, authToken);
                        var previewComments = new ObservableCollection<Comment>(recentComments.Take(2));
                        CommunityRecipes.Add(new CommunityRecipe
                        {
                            Recipe = recipe,
                            Username = user.Username,
                            UserProfilePicture = profilePicture,
                            RecentComments = previewComments
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error loading community recipes: {ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            }
        }

        // Refresh community recipes
        private async Task RefreshCommunityAsync()
        {
            IsRefreshing = true;
            OnPropertyChanged(nameof(IsRefreshing));
            await LoadCommunityRecipesAsync();
            IsRefreshing = false;
            OnPropertyChanged(nameof(IsRefreshing));
        }

        // Get user profile picture or default
        private async Task<string> GetUserProfilePicture(string userId, string authToken)
        {
            try
            {
                var profile = await FirestoreService.GetUserProfileAsync(userId, authToken);
                if (profile != null && !string.IsNullOrEmpty(profile.ProfilePicture))
                    return profile.ProfilePicture;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION in GetUserProfilePicture: {ex.Message}");
            }
            return "default_profile_picture.png";
        }

        // Navigate to comments page for a recipe
        private async void OnCommentClicked(object sender, EventArgs e)
        {
            var button = sender as ImageButton;
            var communityRecipe = button?.CommandParameter as CommunityRecipe;
            if (communityRecipe == null || communityRecipe.Recipe == null)
            {
                await DisplayAlert("Error", "Recipe not selected.", "OK");
                return;
            }
            await Shell.Current.GoToAsync($"{nameof(CommentsPage)}?recipeId={communityRecipe.Recipe.RecipeId}");
        }
    }

    public class CommunityRecipe
    {
        public Recipe Recipe { get; set; }
        public string Username { get; set; }
        public string UserProfilePicture { get; set; }
        public ObservableCollection<Comment> RecentComments { get; set; } = new ObservableCollection<Comment>();

        // Returns ingredients as a comma-separated string
        public string IngredientsString
        {
            get
            {
                if (Recipe?.Ingredients == null || Recipe.Ingredients.Count == 0)
                    return "No ingredients listed.";
                return string.Join(", ", Recipe.Ingredients);
            }
        }
    }
}
