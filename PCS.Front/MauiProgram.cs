using CNAS.Repository.Extensions;
using Microsoft.Extensions.Logging;
using PCS.Auth.Extensions;
using PCS.Business.Garage.Extensions;
using PCS.Front.Shared.Services;
using Serilog;

namespace PCS.Front;

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

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.MongoDB("mongodb://localhost:27017/PCS", "log", Serilog.Events.LogEventLevel.Information)
            .CreateLogger();

        builder.Services
            .AddMongoDb("mongodb://localhost:27017/PCS")
            .AddRepositories()
            .AddGarage()
            .AddAuth0<MainPage>()
            .AddTransient<IDialogService, DialogService>()
            .AddMauiBlazorWebView()
            ;

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}