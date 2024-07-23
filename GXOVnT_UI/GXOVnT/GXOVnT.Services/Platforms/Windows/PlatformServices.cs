using System.Net;
using System.Net.Sockets;

namespace GXOVnT.Services;

// All the code in this file is only included on Windows.
public class PlatformServices
{
    
    public string ReadIpAddress()
    {

        try
        {
            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList
                .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);

            return ipAddress?.ToString() ?? "";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return string.Empty;
        }
    }
}