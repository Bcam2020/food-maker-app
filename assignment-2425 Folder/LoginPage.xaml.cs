using Firebase.Auth;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using assignment_2425.Services;
using assignment_2425.Models;
using System;
using System.Text.RegularExpressions;

namespace assignment_2425
{
    public partial class LoginPage : ContentPage
    {
        private readonly string firebaseApiKey = "AIzaSyAsv973dWPUl6pxSSFMz9VTJjiU0TIEI70";

        public LoginPage()
        {
            InitializeComponent();
        }

        // Apply app theme when page appears
        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.SetAppTheme();
        }

        // Handle back navigation
        private async void OnBackClicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            if (Navigation.ModalStack.Count > 0)
                await Navigation.PopModalAsync();
            else
                await Shell.Current.GoToAsync("//MainPage");
        }

        // Handle user login
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            string email = EmailEntry.Text?.Trim();
            string password = PasswordEntry.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Please enter both email and password.", "OK");
                return;
            }

            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(firebaseApiKey));
                var auth = await authProvider.SignInWithEmailAndPasswordAsync(email, password);

                // Store auth data
                await SecureStorage.SetAsync("auth_token", auth.FirebaseToken);
                await SecureStorage.SetAsync("firebase_user_id", auth.User.LocalId);

                // Ensure user doc exists
                await FirestoreService.EnsureUserDocExistsAsync(auth.User.LocalId, auth.FirebaseToken);

                // Mark user as logged in
                AppShell.IsUserLoggedIn = true;

                // Navigate to ProfilePage
                if (Navigation.ModalStack.Count > 0)
                    await Navigation.PopModalAsync();
                else
                    await Shell.Current.GoToAsync("//ProfilePage");
            }
            catch (FirebaseAuthException ex)
            {
                await DisplayAlert("Login Failed", $"Error: {ex.Reason}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unexpected error: {ex.Message}", "OK");
            }
        }

        // Handle user registration
        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            string email = EmailEntry.Text?.Trim();
            string password = PasswordEntry.Text;

            // Basic validations
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Please enter both email and password.", "OK");
                return;
            }

            // Check email format
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                await DisplayAlert("Error", "Please enter a valid email address.", "OK");
                return;
            }

            // Check password length
            if (password.Length < 6)
            {
                await DisplayAlert("Error", "Password must be at least 6 characters long.", "OK");
                return;
            }

            try
            {
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(firebaseApiKey));
                // Create user with email and password
                var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(email, password);

                // Now that the user is created, log them in automatically
                var signInAuth = await authProvider.SignInWithEmailAndPasswordAsync(email, password);

                // Store the token and user ID
                await SecureStorage.SetAsync("auth_token", signInAuth.FirebaseToken);
                await SecureStorage.SetAsync("firebase_user_id", signInAuth.User.LocalId);

                // Ensure Firestore doc and set user profile
                await FirestoreService.EnsureUserDocExistsAsync(signInAuth.User.LocalId, signInAuth.FirebaseToken);
                var defaultUsername = email.Contains("@") ? email.Substring(0, email.IndexOf("@")) : email;

                var profile = new UserProfile
                {
                    Username = defaultUsername,
                    Email = email,
                    ProfilePicture = ""
                };
                await FirestoreService.SetUserProfileAsync(signInAuth.User.LocalId, profile, signInAuth.FirebaseToken);

                AppShell.IsUserLoggedIn = true;

                await DisplayAlert("Success", "Account created and logged in successfully!", "OK");

                // Navigate directly to ProfilePage
                if (Navigation.ModalStack.Count > 0)
                    await Navigation.PopModalAsync();
                else
                    await Shell.Current.GoToAsync("//ProfilePage");
            }
            catch (FirebaseAuthException ex)
            {
                await DisplayAlert("Registration Failed", $"Error: {ex.Reason}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unexpected error: {ex.Message}", "OK");
            }
        }
    }
}
