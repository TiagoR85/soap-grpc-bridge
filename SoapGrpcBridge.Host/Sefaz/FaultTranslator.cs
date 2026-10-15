using Grpc.Core;
using SoapGrpcBridge.Host.Contracts;

namespace SoapGrpcBridge.Host.Sefaz;

public static class FaultTranslator
{
    public static CoreWCF.FaultException ToFault(RpcException exception) => exception.StatusCode switch
    {
        StatusCode.DeadlineExceeded => SefazFault.Timeout($"backend excedeu o deadline: {exception.Status.Detail}"),
        StatusCode.Unavailable => SefazFault.BackendUnavailable($"backend indisponivel: {exception.Status.Detail}"),
        _ => SefazFault.BackendUnavailable($"erro gRPC {exception.StatusCode}: {exception.Status.Detail}")
    };
}
