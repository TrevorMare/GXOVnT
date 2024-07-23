using System.Net;
using System.Net.Sockets;

namespace GXOVnT.Services;

// All the code in this file is only included on Windows.
public class PlatformClass1
{
    
    public string ReadIpAddress()
    {
        
          
        var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        var ipAddress = ipHostInfo.AddressList[0];
            
            
        var x = ipHostInfo.AddressList.Where(x => x.AddressFamily == AddressFamily.InterNetwork);

        return x.FirstOrDefault().ToString();

    }
}