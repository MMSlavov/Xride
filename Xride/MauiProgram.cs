using Microsoft.Extensions.Logging;
using xRide.Core;

namespace xRide
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            builder.Services.AddMasaBlazor(options =>
            {
                options.ConfigureTheme(theme =>
                {
                    theme.Dark = true;
                });
            });

            builder.Services.AddSingleton<MXService>();

            return builder.Build();
        }
    }
}