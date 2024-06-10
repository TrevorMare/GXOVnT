using GXOVnT.Services.Interfaces;

namespace GXOVnT.Services;

public static class StartupExtensions
{


    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {

        services.AddTransient<IRequestPermissionService, RequestPermissionService>();

        services.AddSingleton<IBluetoothService, BluetoothService>();
        services.AddSingleton<IMessageOrchestrator, MessageOrchestrator>();
        services.AddSingleton<IAlertService, AlertService>();
        services.AddSingleton<ViewModels.LogViewModel>();
        
        
        services.AddTransient<ViewModels.DeviceScannerViewModel>();
        services.AddTransient<ViewModels.EnrollDeviceViewModel>();

        return services;
    }
    
}