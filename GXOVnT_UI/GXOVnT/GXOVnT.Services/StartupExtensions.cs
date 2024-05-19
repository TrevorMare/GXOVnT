using GXOVnT.Services.Interfaces;

namespace GXOVnT.Services;

public static class StartupExtensions
{


    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {

        services.AddTransient<IRequestPermissionService, RequestPermissionService>();
        
        services.AddSingleton<IAlertService, AlertService>();
        services.AddSingleton<ViewModels.LogViewModel>();

        return services;
    }
    
}