using System.ServiceModel;
using SoapGrpcBridge.Host.Contracts;
using SoapGrpcBridge.Tests.Infrastructure;

namespace SoapGrpcBridge.Tests;

public sealed class ValidationWireTests
{
    private static readonly string Chave = new('1', 44);

    [Fact]
    public Task Empty_lote_yields_invalid_request_fault() =>
        AssertInvalidRequest(new AutorizarNfeChannelRequest { cUF = 35, idLote = "1", tpAmb = 2, chaves = [] });

    [Fact]
    public Task Invalid_cUF_yields_invalid_request_fault() =>
        AssertInvalidRequest(new AutorizarNfeChannelRequest { cUF = 99, idLote = "1", tpAmb = 2, chaves = [Chave] });

    [Fact]
    public async Task Invalid_status_cUF_yields_invalid_request_fault()
    {
        using var dead = new DeadBackend();
        var (factory, endpoint) = BridgeFactory.Start(dead.Url);
        using (factory)
        {
            var exception = await Assert.ThrowsAsync<FaultException>(() =>
                SoapChannel.CallAsync(endpoint, channel =>
                    channel.ConsultarStatusServico(new ConsultarStatusServicoChannelRequest { cUF = 99, tpAmb = 2 })));

            AssertInvalidRequestFault(exception);
        }
    }

    private static async Task AssertInvalidRequest(AutorizarNfeChannelRequest request)
    {
        using var dead = new DeadBackend();
        var (factory, endpoint) = BridgeFactory.Start(dead.Url);
        using (factory)
        {
            var exception = await Assert.ThrowsAsync<FaultException>(() =>
                SoapChannel.CallAsync(endpoint, channel => channel.AutorizarNfe(request)));

            AssertInvalidRequestFault(exception);
        }
    }

    private static void AssertInvalidRequestFault(FaultException exception)
    {
        Assert.Equal("Client", exception.Code.Name);
        Assert.Equal(SefazFault.InvalidRequestCode, exception.Code.SubCode?.Name);
        Assert.Contains(SefazFault.InvalidRequestCode, exception.Reason.ToString());
    }
}
