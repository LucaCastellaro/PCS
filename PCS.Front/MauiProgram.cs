﻿using CNAS.Repository.Extensions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using PCS.Auth.Extensions;
using PCS.Business.Garage.Extensions;
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

        builder.Services.AddMauiBlazorWebView();

        builder.Services
            .AddMongoDb("mongodb://localhost:27017/PCS")
            .AddRepositories()
            .AddGarage()
            ;

        builder.Services.TryAddTransient<IDialogService, DialogService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        builder.Services.AddAuth0<MainPage>();

        return builder.Build();
    }
}