using System.Net;
using System.Net.Sockets;

namespace SoapGrpcBridge.Tests.Infrastructure;

internal static class FreePort
{
    public static int Get()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
