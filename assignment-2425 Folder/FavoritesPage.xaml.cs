using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using assignment_2425.Models;
using assignment_2425.Services;
using Microsoft.Maui.Storage;

namespace assignment_2425
{
    public partial class FavoritesPage : ContentPage
    {
        public ObservableCollection<Recipe> FavoriteRecipes { get; set; } = new ObservableCollection<Recipe>();

        public FavoritesPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        // Load favorites when page appears
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            App.SetAppTheme();
            await LoadFavoriteRecipes();
        }

        // Retrieve favorite recipes from Firestore
        private async Task LoadFavoriteRecipes()
        {
            try
            {
                string userId = await SecureStorage.GetAsync("firebase_user_id");
                string authToken = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(authToken))
                {
                    await DisplayAlert("Error", "User not found. Please log in again.", "OK");
                    return;
                }
                var favorites = await FirestoreService.GetUserFavoritesAsync(userId, authToken);
                FavoriteRecipes.Clear();
                foreach (var recipe in favorites)
                {
                    FavoriteRecipes.Add(recipe);
                }
                FavoriteRecipesCollectionView.ItemsSource = FavoriteRecipes;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error loading favorites: {ex.Message}", "OK");
            }
        }

        // Navigate to recipe detail on tap
        private async void OnFavoriteTapped(object sender, EventArgs e)
        {
            if (sender is VisualElement element && element.BindingContext is Recipe selectedRecipe)
            {
                var recipeJson = JsonSerializer.Serialize(selectedRecipe);
                var route = $"{nameof(UserRecipeDetailPage)}?recipeJson={Uri.EscapeDataString(recipeJson)}";
                await Shell.Current.GoToAsync(route);
            }
        }

        // Delete a favorite recipe
        private async void OnDeleteFavoriteInvoked(object sender, EventArgs e)
        {
            if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is Recipe recipe)
            {
                bool confirm = await DisplayAlert("Confirm Delete", $"Are you sure you want to remove \"{recipe.Name}\" from your favorites?", "Yes", "No");
                if (confirm)
                {
                    string userId = await SecureStorage.GetAsync("firebase_user_id");
                    string authToken = await SecureStorage.GetAsync("auth_token");
                    if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(authToken))
                    {
                        try
                        {
                            await FirestoreService.DeleteUserFavoriteAsync(userId, recipe.RecipeId, authToken);
                            FavoriteRecipes.Remove(recipe);
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Error", $"Could not remove favorite: {ex.Message}", "OK");
                        }
                    }
                }
            }
        }
    }
}
