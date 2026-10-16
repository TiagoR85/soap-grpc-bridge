using System.Runtime.Serialization;
using System.ServiceModel;
using SoapGrpcBridge.Host.Contracts;

namespace SoapGrpcBridge.Tests.Infrastructure;

[ServiceContract(Namespace = ContractNames.Namespace, Name = ContractNames.ContractName)]
public interface INfeAuthorizationChannel
{
    [OperationContract(Action = ContractNames.AutorizarNfeAction, ReplyAction = ContractNames.AutorizarNfeReplyAction)]
    Task<AutorizarNfeChannelResponse> AutorizarNfe(AutorizarNfeChannelRequest request);

    [OperationContract(Action = ContractNames.ConsultarStatusServicoAction, ReplyAction = ContractNames.ConsultarStatusServicoReplyAction)]
    Task<ConsultarStatusServicoChannelResponse> ConsultarStatusServico(ConsultarStatusServicoChannelRequest request);
}

[DataContract(Namespace = ContractNames.Namespace, Name = "AutorizarNfeRequest")]
public sealed class AutorizarNfeChannelRequest
{
    [DataMember(Name = "cUF")] public int cUF { get; set; }
    [DataMember(Name = "idLote")] public string idLote { get; set; } = string.Empty;
    [DataMember(Name = "tpAmb")] public int tpAmb { get; set; }
    [DataMember(Name = "chaves")] public string[] chaves { get; set; } = [];
}

[DataContract(Namespace = ContractNames.Namespace, Name = "AutorizarNfeResponseData")]
public sealed class AutorizarNfeChannelResponse
{
    [DataMember(Name = "nRec")] public string nRec { get; set; } = string.Empty;
    [DataMember(Name = "nProt")] public string nProt { get; set; } = string.Empty;
    [DataMember(Name = "cStat")] public int cStat { get; set; }
    [DataMember(Name = "xMotivo")] public string xMotivo { get; set; } = string.Empty;
}

[DataContract(Namespace = ContractNames.Namespace, Name = "ConsultarStatusServicoRequest")]
public sealed class ConsultarStatusServicoChannelRequest
{
    [DataMember(Name = "cUF")] public int cUF { get; set; }
    [DataMember(Name = "tpAmb")] public int tpAmb { get; set; }
}

[DataContract(Namespace = ContractNames.Namespace, Name = "ConsultarStatusServicoResponseData")]
public sealed class ConsultarStatusServicoChannelResponse
{
    [DataMember(Name = "cStat")] public int cStat { get; set; }
    [DataMember(Name = "xMotivo")] public string xMotivo { get; set; } = string.Empty;
    [DataMember(Name = "dhRecbto")] public DateTime dhRecbto { get; set; }
    [DataMember(Name = "versao")] public string versao { get; set; } = string.Empty;
}
