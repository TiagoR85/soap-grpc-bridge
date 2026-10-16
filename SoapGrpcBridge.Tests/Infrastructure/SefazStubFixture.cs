using System.Net.Http;
using WireMock.Server;
using WireMock.Settings;

namespace SoapGrpcBridge.Tests.Infrastructure;

public sealed class SefazStubFixture : IAsyncLifetime
{
    private WireMockServer _server = null!;

    public string GrpcUrl { get; private set; } = string.Empty;
    public WireMockServer Server => _server;
    public string ProtoText { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        ProtoText = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Protos", "sefaz_nfe.proto"));
        _server = WireMockServer.Start(new WireMockServerSettings
        {
            Port = FreePort.Get(),
            UseHttp2 = true,
            Logger = CapturingLogger.Instance
        });
        GrpcUrl = $"http://127.0.0.1:{_server.Port}";

        await WaitForHttp2ReadyAsync(GrpcUrl);
    }

    private static async Task WaitForHttp2ReadyAsync(string grpcUrl, int maxAttempts = 20, int delayMs = 100)
    {
        using var http = new HttpClient { DefaultRequestVersion = System.Net.HttpVersion.Version20 };

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, grpcUrl);
                request.Headers.Connection.Add("keep-alive");
                using var response = await http.SendAsync(request);
                if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NotFound || response.StatusCode == System.Net.HttpStatusCode.MethodNotAllowed || response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return;
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            await Task.Delay(delayMs);
        }

        throw new InvalidOperationException($"WireMock HTTP/2 endpoint at {grpcUrl} did not become ready after {maxAttempts} attempts");
    }

    public Task DisposeAsync()
    {
        _server.Stop();
        _server.Dispose();
        return Task.CompletedTask;
    }
}
