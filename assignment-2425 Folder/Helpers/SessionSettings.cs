using Microsoft.Maui.Storage;

namespace assignment_2425
{
    public static class SessionSettings
    {
        public static bool DarkModeEnabled { get; set; }
        public static double UserTextSize { get; set; }
        public static void Save()
        {
            Preferences.Set("DarkModeEnabled", DarkModeEnabled);
            Preferences.Set("UserTextSize", UserTextSize);
        }
        public static void Load()
        {
            DarkModeEnabled = Preferences.Get("DarkModeEnabled", false);
            UserTextSize = Preferences.Get("UserTextSize", 16.0);
        }
    }
}
