using System.ServiceModel;
using SoapGrpcBridge.Host.Contracts;
using SoapGrpcBridge.Tests.Infrastructure;

namespace SoapGrpcBridge.Tests;

public sealed class StatusWireTests : IClassFixture<SefazStubFixture>
{
    private readonly SefazStubFixture _stub;

    public StatusWireTests(SefazStubFixture stub) => _stub = stub;

    [Fact]
    public async Task Operational_status_returns_cStat_107_with_metadata()
    {
        _stub.Server.ResetMappings();
        SefazStubs.StubStatus(_stub, cStat: 107, xMotivo: "Servico em Operacao", dhRecbto: "2026-10-15T10:00:00Z", versao: "4.00");
        var (factory, endpoint) = BridgeFactory.Start(_stub.GrpcUrl);
        using (factory)
        {
            var response = await SoapChannel.CallAsync(endpoint, channel =>
                channel.ConsultarStatusServico(new ConsultarStatusServicoChannelRequest { cUF = 35, tpAmb = 2 }));

            Assert.Equal(107, response.cStat);
            Assert.Equal("Servico em Operacao", response.xMotivo);
            Assert.Equal("4.00", response.versao);
            Assert.Equal(new DateTime(2026, 10, 15, 10, 0, 0, DateTimeKind.Utc), response.dhRecbto);
        }
    }

    [Fact]
    public async Task Dead_backend_yields_backend_unavailable_fault()
    {
        using var dead = new DeadBackend();
        var (factory, endpoint) = BridgeFactory.Start(dead.Url);
        using (factory)
        {
            var exception = await Assert.ThrowsAsync<FaultException>(() =>
                SoapChannel.CallAsync(endpoint, channel =>
                    channel.ConsultarStatusServico(new ConsultarStatusServicoChannelRequest { cUF = 35, tpAmb = 2 })));

            Assert.Equal("Server", exception.Code.Name);
            Assert.Equal(SefazFault.BackendUnavailableCode, exception.Code.SubCode?.Name);
        }
    }
}
