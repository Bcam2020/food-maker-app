using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;
using CommunityToolkit.Maui;

namespace assignment_2425
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>() // Set the main app class
                .UseMauiCommunityToolkit()  // Enable Community Toolkit features
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Oswald-Bold.ttf", "Oswald-Bold");
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug(); // Enable debug logging in debug mode
#endif

            return builder.Build();
        }
    }
}
