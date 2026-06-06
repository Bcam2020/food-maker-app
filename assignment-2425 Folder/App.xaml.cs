using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace assignment_2425
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public App()
        {
            InitializeComponent();
            var mauiApp = MauiProgram.CreateMauiApp();
            ServiceProvider = mauiApp.Services;
            SessionSettings.Load();
            SetAppTheme();
            MainPage = new AppShell();
        }
        public static void SetAppTheme()
        {
            if (SessionSettings.DarkModeEnabled)
            {
                Current.UserAppTheme = AppTheme.Dark;
                Current.Resources["AppBackgroundColor"] = Colors.Black;
                Current.Resources["AppTextColor"] = Colors.Yellow;
            }
            else
            {
                Current.UserAppTheme = AppTheme.Light;
                Current.Resources["AppBackgroundColor"] = Colors.White;
                Current.Resources["AppTextColor"] = Colors.Black;
            }
            Current.Resources["GlobalFontSize"] = SessionSettings.UserTextSize;
        }
    }
}
