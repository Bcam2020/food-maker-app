using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using assignment_2425.Services;
using assignment_2425.Models;

namespace assignment_2425
{
    public partial class IngredientsPage : ContentPage
    {
        // Collection of user-entered ingredients
        public ObservableCollection<string> Ingredients { get; set; } = new ObservableCollection<string>();

        public IngredientsPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.SetAppTheme();
        }

        // Add a new ingredient to the list
        private void OnAddIngredientClicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            string newIngredient = IngredientEntry.Text?.Trim();
            if (!string.IsNullOrEmpty(newIngredient))
            {
                Ingredients.Add(newIngredient);
                IngredientEntry.Text = "";
            }
        }

        // Remove an ingredient from the list
        private void OnRemoveIngredientClicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            var button = sender as Button;
            string ingredientToRemove = button?.CommandParameter as string;
            if (!string.IsNullOrEmpty(ingredientToRemove) && Ingredients.Contains(ingredientToRemove))
                Ingredients.Remove(ingredientToRemove);
        }

        // Find recipes matching the ingredients and navigate to RecipePage
        private async void OnFindRecipesClicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            if (Ingredients.Count == 0)
            {
                await DisplayAlert("No Ingredients", "Please add at least one ingredient.", "OK");
                return;
            }
            try
            {
                string csvUrl = "https://firebasestorage.googleapis.com/v0/b/assignment-2425.firebasestorage.app/o/Recipes%2Fsample_recipes.csv?alt=media";
                var csvRecipes = await CsvLoader.DownloadAndParseRecipesAsync(csvUrl);
                var matchedRecipes = csvRecipes.Where(recipe =>
                {
                    var recipeIngs = recipe.Ingredients.Select(i => i.ToLower().Trim()).ToList();
                    var userIngs = Ingredients.Select(i => i.ToLower().Trim()).ToList();
                    return userIngs.All(userIng => recipeIngs.Any(rIng => rIng.Contains(userIng)));
                }).ToList();

                if (!matchedRecipes.Any())
                {
                    string debugRecipeNames = string.Join("\n", csvRecipes.Select(r => r.Name));
                    string debugMessage = "No recipes matched your ingredients.\n\n" +
                                          $"Total loaded from CSV: {csvRecipes.Count}\n" +
                                          $"All recipe names:\n{debugRecipeNames}";
                    await DisplayAlert("No Recipes Found", debugMessage, "OK");
                    return;
                }
                await Navigation.PushAsync(new RecipePage(matchedRecipes));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to find recipes: {ex.Message}", "OK");
            }
        }

        // Navigate to the MapPage to get location
        private async void OnGetLocationClicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            await Shell.Current.GoToAsync(nameof(MapPage));
        }

        // Navigate to AddRecipePage to add a new recipe
        private async void OnAddRecipeButtonClicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            await Shell.Current.GoToAsync(nameof(AddRecipePage));
        }

        // Navigate back to MainPage
        private async void OnBackClicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}
