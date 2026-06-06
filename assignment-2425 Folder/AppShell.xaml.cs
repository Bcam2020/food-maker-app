using Microsoft.Maui.Controls;
using System;

namespace assignment_2425
{
    public partial class AppShell : Shell
    {
        // Tracks whether the user is logged in
        public static bool IsUserLoggedIn { get; set; } = false;

        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
            this.Navigating += OnShellNavigating;
        }

        // Register navigation routes
        private void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(IngredientsPage), typeof(IngredientsPage));
            Routing.RegisterRoute(nameof(RecipePage), typeof(RecipePage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(MapPage), typeof(MapPage));
            Routing.RegisterRoute(nameof(AddRecipePage), typeof(AddRecipePage));
            Routing.RegisterRoute(nameof(CameraPage), typeof(CameraPage));
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(FavoritesPage), typeof(FavoritesPage));
            Routing.RegisterRoute(nameof(FriendsListPage), typeof(FriendsListPage));
            Routing.RegisterRoute(nameof(FriendRecipesPage), typeof(FriendRecipesPage));
            Routing.RegisterRoute(nameof(SearchUsersPage), typeof(SearchUsersPage));
            Routing.RegisterRoute(nameof(CommunityPage), typeof(CommunityPage));
            Routing.RegisterRoute(nameof(CommentsPage), typeof(CommentsPage));
            Routing.RegisterRoute(nameof(UserRecipeDetailPage), typeof(UserRecipeDetailPage));
        }

        // Check navigation; if accessing protected pages without login, redirect to LoginPage
        private async void OnShellNavigating(object sender, ShellNavigatingEventArgs e)
        {
            if (e.Target.Location.OriginalString.Contains(nameof(ProfilePage)) ||
                e.Target.Location.OriginalString.Contains(nameof(FavoritesPage)) ||
                e.Target.Location.OriginalString.Contains(nameof(AddRecipePage)))
            {
                if (!IsUserLoggedIn)
                {
                    e.Cancel();
                    await Shell.Current.GoToAsync(nameof(LoginPage));
                }
            }
        }
    }
}
