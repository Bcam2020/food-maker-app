using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Devices;
using Firebase.Storage;
using assignment_2425.Models;
using assignment_2425.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace assignment_2425
{
    public partial class AddRecipePage : ContentPage
    {
        // Stores the URL of the uploaded photo
        private string _photoPath = string.Empty;
        private const string FirebaseStorageBucket = "assignment-2425.firebasestorage.app";

        public AddRecipePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.SetAppTheme();
        }

        // Handle attaching a photo via capture or gallery
        private async void OnAttachPhotoClicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            try
            {
                string action = await DisplayActionSheet("Attach Photo", "Cancel", null, "Take a Photo", "Pick from Gallery");
                FileResult photo = null;
                if (action == "Take a Photo")
                    photo = await MediaPicker.Default.CapturePhotoAsync();
                else if (action == "Pick from Gallery")
                    photo = await MediaPicker.Default.PickPhotoAsync();
                if (photo != null)
                {
                    _photoPath = await UploadPhotoToFirebase(photo);
                    RecipeImage.Source = ImageSource.FromUri(new Uri(_photoPath));
                }
                else
                {
                    _photoPath = string.Empty;
                    RecipeImage.Source = null;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not attach photo: {ex.Message}", "OK");
            }
        }

        // Upload photo to Firebase Storage using a FileResult
        private async Task<string> UploadPhotoToFirebase(FileResult photo)
        {
            using var stream = await photo.OpenReadAsync();
            return await UploadPhotoToFirebase(stream);
        }

        // Upload photo to Firebase Storage using a stream
        private async Task<string> UploadPhotoToFirebase(Stream stream)
        {
            try
            {
                string authToken = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(authToken))
                    throw new Exception("Missing auth token. Please log in again.");
                var storage = new FirebaseStorage(
                    FirebaseStorageBucket,
                    new FirebaseStorageOptions { AuthTokenAsyncFactory = () => Task.FromResult(authToken), ThrowOnCancel = true });
                var fileName = $"{Guid.NewGuid()}.jpg";
                var imageUrl = await storage.Child("recipe_images").Child(fileName).PutAsync(stream);
                return imageUrl;
            }
            catch (Exception ex)
            {
                throw new Exception($"Upload failed: {ex.Message}");
            }
        }

        // Validate inputs and save recipe data to Firestore
        private async void OnSaveRecipeClicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            bool valid = ValidateInputs();
            if (!valid)
                return;

            string userId = await SecureStorage.GetAsync("firebase_user_id");
            if (string.IsNullOrEmpty(userId))
            {
                await DisplayAlert("Error", "Please log in first.", "OK");
                return;
            }
            string authToken = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(authToken))
            {
                await DisplayAlert("Error", "Missing auth token. Please log in again.", "OK");
                return;
            }
            var ingredientsText = RecipeIngredientsEditor.Text ?? "";
            var ingredientsList = ingredientsText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                                 .Select(line => line.Trim())
                                                 .Where(line => !string.IsNullOrEmpty(line))
                                                 .ToList();
            var recipe = new Recipe
            {
                Name = RecipeNameEntry.Text.Trim(),
                Ingredients = ingredientsList,
                Instructions = RecipeInstructionsEditor.Text?.Trim() ?? "",
                ImageUrl = _photoPath
            };
            try
            {
                string recipeId = Guid.NewGuid().ToString("N");
                await FirestoreService.SetUserRecipeAsync(userId, recipeId, recipe, authToken);
                await DisplayAlert("Success", "Recipe saved!", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                bool retry = await DisplayAlert("Error", $"Could not save recipe: {ex.Message}", "Retry", "Cancel");
                if (retry)
                    OnSaveRecipeClicked(sender, e);
            }
        }

        // Check if required fields are filled
        private bool ValidateInputs()
        {
            bool valid = true;
            if (string.IsNullOrWhiteSpace(RecipeNameEntry.Text))
            {
                RecipeNameError.Text = "Recipe name is required.";
                RecipeNameError.IsVisible = true;
                valid = false;
            }
            else
                RecipeNameError.IsVisible = false;
            if (string.IsNullOrWhiteSpace(RecipeIngredientsEditor.Text))
            {
                RecipeIngredientsError.Text = "At least one ingredient is required.";
                RecipeIngredientsError.IsVisible = true;
                valid = false;
            }
            else
                RecipeIngredientsError.IsVisible = false;
            if (string.IsNullOrWhiteSpace(RecipeInstructionsEditor.Text))
            {
                RecipeInstructionsError.Text = "Instructions are required.";
                RecipeInstructionsError.IsVisible = true;
                valid = false;
            }
            else
                RecipeInstructionsError.IsVisible = false;
            return valid;
        }
    }
}
