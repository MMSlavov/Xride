using Xride.Shared;

namespace Xride;

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
        builder.Services.AddMasaBlazor();


        builder.Services.AddSingleton<MXService>();
        builder.Services.AddSingleton<WeatherForecastService>();

        return builder.Build();
    }
}
