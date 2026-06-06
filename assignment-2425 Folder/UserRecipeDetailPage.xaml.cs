using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Text.Json;
using assignment_2425.Models;
using assignment_2425.Services;

namespace assignment_2425
{
    [QueryProperty(nameof(RecipeJson), "recipeJson")]
    public partial class UserRecipeDetailPage : ContentPage
    {
        private string recipeJson;

        public string RecipeJson
        {
            get => recipeJson;
            set
            {
                recipeJson = Uri.UnescapeDataString(value);
                LoadRecipeFromJson(recipeJson);
            }
        }

        public UserRecipeDetailPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.SetAppTheme();
        }

        // Deserialize recipe JSON and update UI.
        void LoadRecipeFromJson(string json)
        {
            try
            {
                var recipe = JsonSerializer.Deserialize<Recipe>(json);
                if (recipe == null) return;

                NameLabel.Text = recipe.Name ?? "No Name";
                IngredientsLabel.Text = string.Join(", ", recipe.Ingredients ?? new List<string>());
                InstructionsLabel.Text = recipe.Instructions ?? "No Instructions";

                // If there's a valid ImageUrl, use it; otherwise, use "placeholder.png".
                if (!string.IsNullOrWhiteSpace(recipe.ImageUrl))
                {
                    RecipeImage.Source = ImageSource.FromUri(new Uri(recipe.ImageUrl));
                }
                else
                {
                    RecipeImage.Source = "placeholder.png";
                }
            }
            catch
            {
                NameLabel.Text = "Error loading recipe details.";
            }
        }

        // Use Text-to-Speech to read out recipe details.
        async void OnReadRecipeClicked(object sender, EventArgs e)
        {
            string text = $"Ingredients: {IngredientsLabel.Text}. Instructions: {InstructionsLabel.Text}";
            await TextToSpeechService.SpeakAsync(text);
        }
    }
}
