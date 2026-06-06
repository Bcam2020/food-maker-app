using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using assignment_2425.Models;
using assignment_2425.Services;

namespace assignment_2425
{
    [QueryProperty(nameof(RecipeId), "recipeId")]
    public partial class CommentsPage : ContentPage
    {
        public string RecipeId { get; set; }
        public ObservableCollection<Comment> Comments { get; set; } = new ObservableCollection<Comment>();

        public CommentsPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        // Load comments when page appears
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            App.SetAppTheme();
            await LoadCommentsAsync();
        }

        // Retrieve comments for the recipe
        private async Task LoadCommentsAsync()
        {
            try
            {
                string authToken = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(authToken))
                {
                    await DisplayAlert("Error", "User not authenticated.", "OK");
                    return;
                }
                var commentsList = await FirestoreService.GetCommentsForRecipeAsync(RecipeId, authToken);
                Comments.Clear();
                foreach (var comment in commentsList)
                {
                    Comments.Add(comment);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load comments: {ex.Message}", "OK");
            }
        }

        // Send a new comment
        private async void OnSendCommentClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CommentEntry.Text))
                return;
            try
            {
                string authToken = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(authToken))
                {
                    await DisplayAlert("Error", "User not authenticated.", "OK");
                    return;
                }
                string userName = await SecureStorage.GetAsync("user_name") ?? "Anonymous";
                Comment newComment = new Comment
                {
                    RecipeId = RecipeId,
                    UserName = userName,
                    Text = CommentEntry.Text,
                    Timestamp = DateTime.UtcNow
                };
                await FirestoreService.AddCommentAsync(RecipeId, newComment, authToken);
                CommentEntry.Text = "";
                await LoadCommentsAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to add comment: {ex.Message}", "OK");
            }
        }
    }
}
