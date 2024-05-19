using GXOVnT.Services.Interfaces;

namespace GXOVnT.Services;

public static class StartupExtensions
{


    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {

        services.AddTransient<IRequestPermissionService, RequestPermissionService>();
        services.AddTransient<IBluetoothService, BluetoothService>();
        
        services.AddSingleton<IAlertService, AlertService>();
        
        services.AddSingleton<ViewModels.LogViewModel>();
        services.AddTransient<ViewModels.BLEScannerViewModel>();

        return services;
    }
    
}