//https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/appmodel/permissions?view=net-maui-8.0&tabs=android

using System.Net;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using GXOVnT.Shared.Common;

namespace GXOVnT.Services;

// All the code in this file is only included on Android.
public class PlatformServices
{

    #region Android Methods

    public string ReadIpAddress()
    {
        try
        {

#if !ANDROID
            return string.Empty;    
#endif

            var result = string.Empty;

            if (OperatingSystem.IsAndroidVersionAtLeast(23))
            {
                var connectivityManager = (ConnectivityManager?)Platform.AppContext.GetSystemService(Context.ConnectivityService);
                if (connectivityManager == null)
                    throw new GXOVnTException("Could not load the connectivity manager");

                var linkProperties = connectivityManager.GetLinkProperties(connectivityManager.ActiveNetwork);
                if (linkProperties == null)
                    throw new GXOVnTException("Could not load the active network");

                var linkAddress = linkProperties.LinkAddresses
                    .FirstOrDefault(a => a.Address is Java.Net.Inet4Address);
                
                if (linkAddress == null)
                    throw new GXOVnTException("Could not locate an IPv4 Address");

                result = ((Java.Net.Inet4Address)linkAddress.Address!).HostAddress ?? "";
            }
            else if (OperatingSystem.IsAndroidVersionAtLeast(21))
            {
                var wifiManager = (WifiManager?)Android.App.Application.Context.GetSystemService(Context.WifiService);
                if (wifiManager?.ConnectionInfo == null)
                    throw new GXOVnTException("Could not initialize the WiFi manager");

                var wifiManagerIpAddress = wifiManager.ConnectionInfo.IpAddress;
                var ipAddr = new IPAddress(wifiManagerIpAddress);
                result = ipAddr.ToString();
            }

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return string.Empty;
        }
    }

    #endregion
    
   
   
}