using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using System;
using System.Collections.Generic;
using assignment_2425.Models;
using assignment_2425.Services;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace assignment_2425
{
    public partial class RecipePage : ContentPage
    {
        public List<Recipe> Recipes { get; set; }

        public RecipePage()
        {
            InitializeComponent();
        }

        public RecipePage(List<Recipe> matchedRecipes) : this()
        {
            Recipes = matchedRecipes;
            BindingContext = this;
        }

        // Handle favorite button click
        async void OnFavoriteClicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);

            // Check if the user is logged in before adding to favorites.
            if (!AppShell.IsUserLoggedIn)
            {
                bool login = await DisplayAlert("Not Logged In", "Please log in to add recipes to favorites. Would you like to log in now?", "Yes", "No");
                if (login)
                {
                    await Shell.Current.GoToAsync(nameof(LoginPage));
                }
                return;
            }

            // Ensure the sender has a valid recipe in its CommandParameter.
            if (sender is Button btn && btn.CommandParameter is Recipe selectedRecipe)
            {
                string userId = await SecureStorage.GetAsync("firebase_user_id");
                string authToken = await SecureStorage.GetAsync("auth_token");

                try
                {
                    // Call FirestoreService to add the favorite.
                    await FirestoreService.AddUserFavoriteAsync(userId, selectedRecipe.RecipeId, selectedRecipe, authToken);
                    await DisplayAlert("Favorite", "Recipe added to favorites!", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Could not add favorite: {ex.Message}", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "Could not determine the selected recipe.", "OK");
            }
        }

        // Handle back button click
        async void OnBackClicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            await Shell.Current.GoToAsync("IngredientsPage");
        }

        // Read recipe instructions aloud
        async void OnReadInstructionsClicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            var recipe = (Recipe)((TappedEventArgs)e).Parameter;
            if (recipe == null)
                return;
            string text = "Ingredients: " + string.Join(", ", recipe.Ingredients) + ". Instructions: " + recipe.Instructions;
            await TextToSpeechService.SpeakAsync(text);
        }
    }
}
