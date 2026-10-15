using WireMock.Server;
using WireMock.Settings;

namespace SoapGrpcBridge.Tests.Infrastructure;

public sealed class SefazStubFixture : IAsyncLifetime
{
    private WireMockServer _server = null!;

    public string GrpcUrl { get; private set; } = string.Empty;
    public WireMockServer Server => _server;
    public string ProtoText { get; private set; } = string.Empty;

    public Task InitializeAsync()
    {
        ProtoText = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Protos", "sefaz_nfe.proto"));
        _server = WireMockServer.Start(new WireMockServerSettings
        {
            Port = FreePort.Get(),
            UseHttp2 = true,
            Logger = CapturingLogger.Instance
        });
        GrpcUrl = $"http://127.0.0.1:{_server.Port}";
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _server.Stop();
        _server.Dispose();
        return Task.CompletedTask;
    }
}
