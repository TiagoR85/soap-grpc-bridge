using SoapGrpcBridge.Host.Contracts;
using SoapGrpcBridge.Host.Mapping;
using Proto = SoapGrpcBridge.Contracts.Sefaz.Nfe.V1;

namespace SoapGrpcBridge.Tests.Mapping;

public sealed class NfeMapperTests
{
    [Fact]
    public void ToProto_maps_authorization_request_fields()
    {
        var dto = new AutorizarNfeRequest { cUF = 35, idLote = "42", tpAmb = 2, chaves = ["1", "2"] };

        var proto = NfeMapper.ToProto(dto);

        Assert.Equal(35, proto.CUF);
        Assert.Equal("42", proto.IdLote);
        Assert.Equal(2, proto.TpAmb);
        Assert.Equal(new[] { "1", "2" }, proto.Chaves);
    }

    [Fact]
    public void ToProto_maps_empty_chaves_to_empty_repeated_field()
    {
        var dto = new AutorizarNfeRequest { cUF = 35, idLote = "1", tpAmb = 1, chaves = [] };

        var proto = NfeMapper.ToProto(dto);

        Assert.Empty(proto.Chaves);
    }

    [Fact]
    public void ToResponse_maps_authorized_result()
    {
        var proto = new Proto.AutorizarNfeResponse { NRec = "9001", NProt = "123", CStat = 100, XMotivo = "Autorizado o uso da NF-e" };

        var dto = NfeMapper.ToResponse(proto);

        Assert.Equal("9001", dto.nRec);
        Assert.Equal("123", dto.nProt);
        Assert.Equal(100, dto.cStat);
        Assert.Equal("Autorizado o uso da NF-e", dto.xMotivo);
    }

    [Fact]
    public void ToResponse_keeps_protocol_fields_empty_when_rejected()
    {
        var proto = new Proto.AutorizarNfeResponse { CStat = 204, XMotivo = "Rejeicao: duplicidade" };

        var dto = NfeMapper.ToResponse(proto);

        Assert.Equal(204, dto.cStat);
        Assert.Equal(string.Empty, dto.nRec);
        Assert.Equal(string.Empty, dto.nProt);
    }

    [Fact]
    public void ToProto_maps_status_request_fields()
    {
        var dto = new ConsultarStatusServicoRequest { cUF = 35, tpAmb = 2 };

        var proto = NfeMapper.ToProto(dto);

        Assert.Equal(35, proto.CUF);
        Assert.Equal(2, proto.TpAmb);
    }

    [Fact]
    public void ToResponse_maps_status_result_with_dhRecbto_roundtrip()
    {
        var proto = new Proto.ConsultarStatusServicoResponse
        {
            CStat = 107,
            XMotivo = "Servico em Operacao",
            DhRecbto = "2026-10-15T10:00:00Z",
            Versao = "4.00"
        };

        var dto = NfeMapper.ToResponse(proto);

        Assert.Equal(107, dto.cStat);
        Assert.Equal("Servico em Operacao", dto.xMotivo);
        Assert.Equal("4.00", dto.versao);
        Assert.Equal(new DateTime(2026, 10, 15, 10, 0, 0, DateTimeKind.Utc), dto.dhRecbto);
    }
}
