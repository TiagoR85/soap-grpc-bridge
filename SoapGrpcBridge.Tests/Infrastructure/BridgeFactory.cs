using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace SoapGrpcBridge.Tests.Infrastructure;

internal static class BridgeFactory
{
    public static (WebApplicationFactory<Program> Factory, Uri Endpoint) Start(string grpcUrl, int deadlineSeconds = 1)
    {
        var settings = new Dictionary<string, string?>
        {
            ["Sefaz:GrpcUrl"] = grpcUrl,
            ["Sefaz:DeadlineSeconds"] = deadlineSeconds.ToString(CultureInfo.InvariantCulture)
        };
        var port = FreePort.Get();

        WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseUrls($"http://127.0.0.1:{port}");
                builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(settings));
            });

        factory.UseKestrel();
        using var http = factory.CreateClient();
        return (factory, new Uri(http.BaseAddress!, "nfe"));
    }
}
