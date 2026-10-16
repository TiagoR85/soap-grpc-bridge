using Microsoft.AspNetCore.Mvc.Testing;

namespace SoapGrpcBridge.Tests;

public sealed class HostSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HostSmokeTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task Health_endpoint_reports_ok()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");

        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, $"status={(int)response.StatusCode} body={body}");
        Assert.Equal("\"ok\"", body);
    }
}
