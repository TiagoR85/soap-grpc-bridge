using System.Net;
using System.Net.Sockets;

namespace SoapGrpcBridge.Tests.Infrastructure;

internal sealed class DeadBackend : IDisposable
{
    private readonly TcpListener _listener;

    public string Url { get; }

    public DeadBackend()
    {
        _listener = new TcpListener(IPAddress.Loopback, 0);
        _listener.Start();
        Url = $"http://127.0.0.1:{((IPEndPoint)_listener.LocalEndpoint).Port}";
        _ = Task.Run(async () =>
        {
            while (true)
            {
                TcpClient client;
                try
                {
                    client = await _listener.AcceptTcpClientAsync();
                }
                catch
                {
                    return;
                }
                client.Dispose();
            }
        });
    }

    public void Dispose()
    {
        try
        {
            _listener.Stop();
        }
        catch
        {
            // listener already stopped
        }
    }
}
