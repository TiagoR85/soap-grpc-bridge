using System.ServiceModel;
using SoapGrpcBridge.Host.Contracts;
using SoapGrpcBridge.Tests.Infrastructure;

namespace SoapGrpcBridge.Tests;

public sealed class AuthorizationWireTests : IClassFixture<SefazStubFixture>
{
    private static readonly string Chave = new('1', 44);
    private readonly SefazStubFixture _stub;

    public AuthorizationWireTests(SefazStubFixture stub) => _stub = stub;

    [Fact]
    public async Task Success_returns_receipt_and_protocol()
    {
        _stub.Server.ResetMappings();
        SefazStubs.StubAutorizar(_stub, cStat: 100, nRec: "9001", nProt: "123", xMotivo: "Autorizado o uso da NF-e");
        var (factory, endpoint) = BridgeFactory.Start(_stub.GrpcUrl);
        using (factory)
        {
            var response = await SoapChannel.CallAsync(endpoint, channel =>
                channel.AutorizarNfe(new AutorizarNfeChannelRequest { cUF = 35, idLote = "1", tpAmb = 2, chaves = [Chave] }));

            Assert.Equal(100, response.cStat);
            Assert.Equal("9001", response.nRec);
            Assert.Equal("123", response.nProt);
            Assert.Equal("Autorizado o uso da NF-e", response.xMotivo);
        }
    }

    [Fact]
    public async Task Business_rejection_travels_as_normal_response_not_fault()
    {
        _stub.Server.ResetMappings();
        SefazStubs.StubAutorizar(_stub, cStat: 204, nRec: "", nProt: "", xMotivo: "Rejeicao: duplicidade");
        var (factory, endpoint) = BridgeFactory.Start(_stub.GrpcUrl);
        using (factory)
        {
            var response = await SoapChannel.CallAsync(endpoint, channel =>
                channel.AutorizarNfe(new AutorizarNfeChannelRequest { cUF = 35, idLote = "1", tpAmb = 2, chaves = [Chave] }));

            Assert.Equal(204, response.cStat);
            Assert.Equal("Rejeicao: duplicidade", response.xMotivo);
            Assert.Equal(string.Empty, response.nProt);
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
                    channel.AutorizarNfe(new AutorizarNfeChannelRequest { cUF = 35, idLote = "1", tpAmb = 2, chaves = [Chave] })));

            Assert.Equal("Server", exception.Code.Name);
            Assert.Equal(SefazFault.BackendUnavailableCode, exception.Code.SubCode?.Name);
            Assert.Contains(SefazFault.BackendUnavailableCode, exception.Reason.ToString());
        }
    }

    [Fact]
    public async Task Slow_backend_yields_timeout_fault()
    {
        _stub.Server.ResetMappings();
        SefazStubs.StubAutorizar(_stub, cStat: 100, nRec: "9001", nProt: "123", xMotivo: "ok", delayMilliseconds: 3000);
        var (factory, endpoint) = BridgeFactory.Start(_stub.GrpcUrl, deadlineSeconds: 1);
        using (factory)
        {
            var exception = await Assert.ThrowsAsync<FaultException>(() =>
                SoapChannel.CallAsync(endpoint, channel =>
                    channel.AutorizarNfe(new AutorizarNfeChannelRequest { cUF = 35, idLote = "1", tpAmb = 2, chaves = [Chave] })));

            Assert.Equal("Server", exception.Code.Name);
            Assert.Equal(SefazFault.TimeoutCode, exception.Code.SubCode?.Name);
        }
    }

    [Fact]
    public async Task Response_without_required_protocol_yields_integrity_fault()
    {
        _stub.Server.ResetMappings();
        SefazStubs.StubAutorizar(_stub, cStat: 100, nRec: "", nProt: "", xMotivo: "Autorizado o uso da NF-e");
        var (factory, endpoint) = BridgeFactory.Start(_stub.GrpcUrl);
        using (factory)
        {
            var exception = await Assert.ThrowsAsync<FaultException>(() =>
                SoapChannel.CallAsync(endpoint, channel =>
                    channel.AutorizarNfe(new AutorizarNfeChannelRequest { cUF = 35, idLote = "1", tpAmb = 2, chaves = [Chave] })));

            Assert.Equal("Server", exception.Code.Name);
            Assert.Equal(SefazFault.IntegrityErrorCode, exception.Code.SubCode?.Name);
            Assert.Contains(SefazFault.IntegrityErrorCode, exception.Reason.ToString());
        }
    }
}
