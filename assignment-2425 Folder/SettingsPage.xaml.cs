using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.ApplicationModel; // For Permissions API
using System;
using System.Threading.Tasks;

namespace assignment_2425
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        // Update UI with current settings when the page appears
        protected override void OnAppearing()
        {
            base.OnAppearing();
            SessionSettings.Load();
            DarkModeSwitch.IsToggled = SessionSettings.DarkModeEnabled;
            double userTextSize = SessionSettings.UserTextSize;
            if (userTextSize < 16)
                SmallTextRadio.IsChecked = true;
            else if (userTextSize < 20)
                MediumTextRadio.IsChecked = true;
            else
                LargeTextRadio.IsChecked = true;
            App.SetAppTheme();
        }

        // Toggle dark mode and update theme
        void OnDarkModeToggled(object sender, ToggledEventArgs e)
        {
            SessionSettings.DarkModeEnabled = e.Value;
            App.SetAppTheme();
        }

        // Toggle flashlight with runtime permission checking
        async void OnFlashlightToggled(object sender, ToggledEventArgs e)
        {
            try
            {
                if (e.Value)
                {
                    bool granted = await CheckAndRequestCameraPermissionAsync();
                    if (!granted)
                    {
                        await DisplayAlert("Permission Denied", "Camera permission is required to use the flashlight.", "OK");
                        return;
                    }
                    await Flashlight.Default.TurnOnAsync();
                    await DisplayAlert("Secret Recipe", "You've discovered a hidden recipe!", "Awesome!");
                }
                else
                {
                    await Flashlight.Default.TurnOffAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not toggle flashlight: {ex.Message}", "OK");
            }
        }

        // Request camera permission at runtime
        private async Task<bool> CheckAndRequestCameraPermissionAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }
            return status == PermissionStatus.Granted;
        }

        // Update text size based on selected radio button
        void OnTextSizeRadioChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!e.Value)
                return;
            RadioButton rb = sender as RadioButton;
            double newSize = 18;
            if (rb == SmallTextRadio)
                newSize = 14;
            else if (rb == MediumTextRadio)
                newSize = 18;
            else if (rb == LargeTextRadio)
                newSize = 22;
            SessionSettings.UserTextSize = newSize;
            App.SetAppTheme();
        }

        // Save current settings
        async void OnSaveClicked(object sender, EventArgs e)
        {
            SessionSettings.Save();
            await DisplayAlert("Settings", "Settings have been saved", "OK");
        }

        // Navigate back to the Main Page
        async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}
