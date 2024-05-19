using GXOVnT.Services;
using Microsoft.Extensions.Logging;

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
        

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
    
  
}