using Grpc.Core;
using SoapGrpcBridge.Host.Sefaz;
using SoapGrpcBridge.Tests.Infrastructure;
using Proto = SoapGrpcBridge.Contracts.Sefaz.Nfe.V1;

namespace SoapGrpcBridge.Tests.Sefaz;

public sealed class SefazGrpcClientTests : IClassFixture<SefazStubFixture>
{
    private readonly SefazStubFixture _stub;

    public SefazGrpcClientTests(SefazStubFixture stub) => _stub = stub;

    [Fact]
    public async Task AutorizarNfe_returns_stubbed_response()
    {
        _stub.Server.ResetMappings();
        SefazStubs.StubAutorizar(_stub, cStat: 100, nRec: "9001", nProt: "123", xMotivo: "Autorizado o uso da NF-e");
        using var client = new SefazGrpcClient(_stub.GrpcUrl, deadlineSeconds: 2);

        var response = await client.AutorizarNfeAsync(new Proto.AutorizarNfeRequest
        {
            CUF = 35,
            IdLote = "1",
            TpAmb = 2,
            Chaves = { new string('1', 44) }
        });

        Assert.Equal(100, response.CStat);
        Assert.Equal("9001", response.NRec);
        Assert.Equal("123", response.NProt);
    }

    [Fact]
    public async Task AutorizarNfe_dead_port_throws_unavailable()
    {
        using var dead = new DeadBackend();
        using var client = new SefazGrpcClient(dead.Url, deadlineSeconds: 2);

        var exception = await Assert.ThrowsAsync<RpcException>(() =>
            client.AutorizarNfeAsync(new Proto.AutorizarNfeRequest { CUF = 35, IdLote = "1", TpAmb = 2, Chaves = { "1" } }));

        Assert.Equal(StatusCode.Unavailable, exception.StatusCode);
    }

    [Fact]
    public async Task AutorizarNfe_delay_over_deadline_throws_deadline_exceeded()
    {
        _stub.Server.ResetMappings();
        SefazStubs.StubAutorizar(_stub, cStat: 100, nRec: "9001", nProt: "123", xMotivo: "ok", delayMilliseconds: 3000);
        using var client = new SefazGrpcClient(_stub.GrpcUrl, deadlineSeconds: 1);

        var exception = await Assert.ThrowsAsync<RpcException>(() =>
            client.AutorizarNfeAsync(new Proto.AutorizarNfeRequest { CUF = 35, IdLote = "1", TpAmb = 2, Chaves = { "1" } }));

        Assert.Equal(StatusCode.DeadlineExceeded, exception.StatusCode);
    }

    [Fact]
    public async Task ConsultarStatusServico_returns_stubbed_status()
    {
        _stub.Server.ResetMappings();
        SefazStubs.StubStatus(_stub, cStat: 107, xMotivo: "Servico em Operacao", dhRecbto: "2026-10-15T10:00:00Z", versao: "4.00");
        using var client = new SefazGrpcClient(_stub.GrpcUrl, deadlineSeconds: 2);

        var response = await client.ConsultarStatusServicoAsync(new Proto.ConsultarStatusServicoRequest { CUF = 35, TpAmb = 2 });

        Assert.Equal(107, response.CStat);
        Assert.Equal("4.00", response.Versao);
    }
}
