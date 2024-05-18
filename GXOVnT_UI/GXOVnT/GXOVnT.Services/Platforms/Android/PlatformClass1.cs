//https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/appmodel/permissions?view=net-maui-8.0&tabs=android
namespace GXOVnT.Services;

// All the code in this file is only included on Android.
public class PlatformClass1
{

    public async Task GetBluetoothPermission()
    {
        
        // PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();
        // PermissionStatus status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
    }
    
    public async Task<PermissionStatus> CheckAndRequestLocationPermission()
    {
        PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if (status == PermissionStatus.Granted)
            return status;

        if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
        {
            // Prompt the user to turn on in settings
            // On iOS once a permission has been denied it may not be requested again from the application
            return status;
        }

        if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
        {
            // Prompt the user with additional information as to why the permission is needed
        }

        status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

        return status;
    }
}