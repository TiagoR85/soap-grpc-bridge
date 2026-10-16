using SoapGrpcBridge.Tests.Infrastructure;

namespace SoapGrpcBridge.Tests;

public sealed class WsdlTests
{
    [Fact]
    public async Task Get_wsdl_exposes_both_operations()
    {
        using var dead = new DeadBackend();
        var (factory, endpoint) = BridgeFactory.Start(dead.Url);
        using (factory)
        {
            using var http = factory.CreateClient();
            var xml = await http.GetStringAsync(new Uri(endpoint, "?wsdl"));

            Assert.Contains("<wsdl:definitions", xml);
            Assert.Contains("AutorizarNfe", xml);
            Assert.Contains("ConsultarStatusServico", xml);
        }
    }
}
