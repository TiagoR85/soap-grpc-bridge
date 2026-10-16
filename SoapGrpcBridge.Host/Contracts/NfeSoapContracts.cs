using System.Runtime.Serialization;

namespace SoapGrpcBridge.Host.Contracts;

public static class ContractNames
{
    public const string Namespace = "http://www.portalfiscal.inf.br/nfe/wsdl/NFeAutorizacao4";
    public const string ContractName = "NFeAutorizacao4";
    public const string AutorizarNfeAction = Namespace + "/" + ContractName + "/AutorizarNfe";
    public const string AutorizarNfeReplyAction = Namespace + "/" + ContractName + "/AutorizarNfeResponse";
    public const string ConsultarStatusServicoAction = Namespace + "/" + ContractName + "/ConsultarStatusServico";
    public const string ConsultarStatusServicoReplyAction = Namespace + "/" + ContractName + "/ConsultarStatusServicoResponse";
}

[CoreWCF.ServiceContract(Namespace = ContractNames.Namespace, Name = ContractNames.ContractName)]
public interface INfeAuthorizationService
{
    [CoreWCF.OperationContract(Action = ContractNames.AutorizarNfeAction, ReplyAction = ContractNames.AutorizarNfeReplyAction)]
    Task<AutorizarNfeResponse> AutorizarNfe(AutorizarNfeRequest request);

    [CoreWCF.OperationContract(Action = ContractNames.ConsultarStatusServicoAction, ReplyAction = ContractNames.ConsultarStatusServicoReplyAction)]
    Task<ConsultarStatusServicoResponse> ConsultarStatusServico(ConsultarStatusServicoRequest request);
}

[DataContract(Namespace = ContractNames.Namespace, Name = "AutorizarNfeRequest")]
public sealed class AutorizarNfeRequest
{
    [DataMember(Name = "cUF")] public int cUF { get; set; }
    [DataMember(Name = "idLote")] public string idLote { get; set; } = string.Empty;
    [DataMember(Name = "tpAmb")] public int tpAmb { get; set; }
    [DataMember(Name = "chaves")] public string[] chaves { get; set; } = [];
}

// Nome do DataContract NAO pode ser "AutorizarNfeResponse": o CoreWCF gera o
// wrapper de resposta como operationName + "Response" no mesmo namespace,
// causando "global element already been declared" na exportacao do WSDL.
[DataContract(Namespace = ContractNames.Namespace, Name = "AutorizarNfeResponseData")]
public sealed class AutorizarNfeResponse
{
    [DataMember(Name = "nRec")] public string nRec { get; set; } = string.Empty;
    [DataMember(Name = "nProt")] public string nProt { get; set; } = string.Empty;
    [DataMember(Name = "cStat")] public int cStat { get; set; }
    [DataMember(Name = "xMotivo")] public string xMotivo { get; set; } = string.Empty;
}

[DataContract(Namespace = ContractNames.Namespace, Name = "ConsultarStatusServicoRequest")]
public sealed class ConsultarStatusServicoRequest
{
    [DataMember(Name = "cUF")] public int cUF { get; set; }
    [DataMember(Name = "tpAmb")] public int tpAmb { get; set; }
}

[DataContract(Namespace = ContractNames.Namespace, Name = "ConsultarStatusServicoResponseData")]
public sealed class ConsultarStatusServicoResponse
{
    [DataMember(Name = "cStat")] public int cStat { get; set; }
    [DataMember(Name = "xMotivo")] public string xMotivo { get; set; } = string.Empty;
    [DataMember(Name = "dhRecbto")] public DateTime dhRecbto { get; set; }
    [DataMember(Name = "versao")] public string versao { get; set; } = string.Empty;
}
