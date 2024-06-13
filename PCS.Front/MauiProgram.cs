using Microsoft.Extensions.Logging;
using PCS.Auth.Extensions;
using PCS.Business.Garage.Extensions;
using PCS.Business.Refuel.Extensions;
using PCS.Common.Business.Extensions;
using PCS.Front.Shared.Services;

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

        builder.Services
            .AddMongoDb("mongodb://localhost:27017/PCS")
            .AddRepositories()
            .AddGarage()
            .AddRefuel()
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