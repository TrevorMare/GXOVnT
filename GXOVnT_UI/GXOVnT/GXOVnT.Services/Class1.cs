using GXOVnT.Services.Interfaces;

namespace GXOVnT.Services;

//https://devblogs.microsoft.com/xamarin/requesting-runtime-permissions-in-android-marshmallow/
// All the code in this file is included in all platforms.
public static class Class1
{
    
    private static IServiceProvider ServicesProvider;
    public static IServiceProvider Services => ServicesProvider;
    private static IAlertService AlertService;
    public static IAlertService AlertSvc => AlertService;
    public static LogService Logger = new LogService();

    public static void Initialize(IServiceProvider provider)
    {
        ServicesProvider = provider;
        AlertService = Services.GetService<IAlertService>();

        PlatformClass1 p1 = new PlatformClass1();
        
        
    }
    
    
    
}