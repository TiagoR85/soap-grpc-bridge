using Grpc.Core;
using Microsoft.Extensions.Logging;
using SoapGrpcBridge.Host.Contracts;

namespace SoapGrpcBridge.Host.Sefaz;

public sealed class FaultTranslator
{
    private readonly ILogger<FaultTranslator> _logger;

    public FaultTranslator(ILogger<FaultTranslator> logger)
    {
        _logger = logger;
    }

    public CoreWCF.FaultException ToFault(RpcException exception)
    {
        return exception.StatusCode switch
        {
            StatusCode.DeadlineExceeded => SefazFault.Timeout($"backend excedeu o deadline: {exception.Status.Detail}"),
            StatusCode.Unavailable => SefazFault.BackendUnavailable($"backend indisponivel: {exception.Status.Detail}"),
            _ => HandleCatchAll(exception)
        };
    }

    private CoreWCF.FaultException HandleCatchAll(RpcException exception)
    {
        _logger.LogWarning("Unhandled gRPC status {StatusCode} mapped to BackendUnavailable: {Detail}", exception.StatusCode, exception.Status.Detail);
        return SefazFault.BackendUnavailable($"erro gRPC {exception.StatusCode}: {exception.Status.Detail}");
    }
}
