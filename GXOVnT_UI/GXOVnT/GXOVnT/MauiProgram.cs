using GXOVnT.Services;
using GXOVnT.Services.Wizards;
using GXOVnT.Shared;
using Microsoft.Extensions.Logging;
using MudBlazor.Extensions;

namespace GXOVnT;

public static class MauiProgram
{

    #region Properties

    public static bool IsAndroid => DeviceInfo.Current.Platform == DevicePlatform.Android;

    public static bool IsMacCatalyst => DeviceInfo.Current.Platform == DevicePlatform.MacCatalyst;

    public static bool IsMacOs => DeviceInfo.Current.Platform == DevicePlatform.macOS;

    #endregion
    
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.RegisterServices();
        //
        // builder.Services.AddTransient<CheckBTPermissionsVM>();
        builder.Services.AddTransient<EnrollDeviceWizardSchema>();
        
        // use this to add MudServices and the MudBlazor.Extensions
        builder.Services.AddMudServicesWithExtensions(c => c.WithoutAutomaticCssLoading());

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        AppService.SetServiceProvider(app.Services);

        return app;
    }
    
  
}