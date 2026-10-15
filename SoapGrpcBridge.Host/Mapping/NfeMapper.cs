using System.Globalization;
using SoapGrpcBridge.Host.Contracts;
using Proto = SoapGrpcBridge.Contracts.Sefaz.Nfe.V1;

namespace SoapGrpcBridge.Host.Mapping;

public static class NfeMapper
{
    public static Proto.AutorizarNfeRequest ToProto(AutorizarNfeRequest request)
    {
        var proto = new Proto.AutorizarNfeRequest
        {
            CUF = request.cUF,
            IdLote = request.idLote,
            TpAmb = request.tpAmb
        };
        proto.Chaves.AddRange(request.chaves);
        return proto;
    }

    public static AutorizarNfeResponse ToResponse(Proto.AutorizarNfeResponse proto) => new()
    {
        nRec = proto.NRec,
        nProt = proto.NProt,
        cStat = proto.CStat,
        xMotivo = proto.XMotivo
    };

    public static Proto.ConsultarStatusServicoRequest ToProto(ConsultarStatusServicoRequest request) => new()
    {
        CUF = request.cUF,
        TpAmb = request.tpAmb
    };

    public static ConsultarStatusServicoResponse ToResponse(Proto.ConsultarStatusServicoResponse proto) => new()
    {
        cStat = proto.CStat,
        xMotivo = proto.XMotivo,
        dhRecbto = string.IsNullOrEmpty(proto.DhRecbto)
            ? default
            : DateTime.Parse(proto.DhRecbto, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
        versao = proto.Versao
    };
}
