﻿using GXOVnT.Services.Interfaces;
using GXOVnT.Services.Services;

namespace GXOVnT.Services;

public static class StartupExtensions
{


    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {

        services.AddTransient<IRequestPermissionService, RequestPermissionService>();

        services.AddSingleton<IBluetoothService, BluetoothService>();
        services.AddSingleton<IMessageOrchestrator, MessageOrchestrator>();
        services.AddSingleton<IAlertService, AlertService>();
        services.AddSingleton<ILogService, LogService>();
        services.AddTransient<IGXOVnTBleDeviceService, GXOVnTBleDeviceService>();

        return services;
    }
    
}