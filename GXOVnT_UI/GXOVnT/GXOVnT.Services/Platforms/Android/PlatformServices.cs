//https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/appmodel/permissions?view=net-maui-8.0&tabs=android

using System.Net;
using System.Runtime.Serialization;
using Android.App;
using Android.Net.Wifi;



namespace GXOVnT.Services;

// All the code in this file is only included on Android.
public class PlatformClass1
{

    public string ReadIpAddress()
    {
        
        WifiManager wifiManager = (WifiManager)Android.App.Application.Context.GetSystemService(Service.WifiService);
        int ipaddress = wifiManager.ConnectionInfo.IpAddress;
        IPAddress ipAddr = new IPAddress(ipaddress);
        //  System.out.println(host);  
        return ipAddr.ToString();
        
        
    }
   
}