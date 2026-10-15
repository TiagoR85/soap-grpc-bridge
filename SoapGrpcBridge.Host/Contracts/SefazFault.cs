using CoreWCF;

namespace SoapGrpcBridge.Host.Contracts;

public static class SefazFault
{
    public const string BackendUnavailableCode = "SEFAZ:BackendUnavailable";
    public const string TimeoutCode = "SEFAZ:Timeout";
    public const string IntegrityErrorCode = "SEFAZ:IntegrityError";
    public const string InvalidRequestCode = "SEFAZ:InvalidRequest";

    public static FaultException InvalidRequest(string detail) => Create("Client", InvalidRequestCode, detail);

    public static FaultException BackendUnavailable(string detail) => Create("Server", BackendUnavailableCode, detail);

    public static FaultException Timeout(string detail) => Create("Server", TimeoutCode, detail);

    public static FaultException IntegrityError(string detail) => Create("Server", IntegrityErrorCode, detail);

    private static FaultException Create(string level, string code, string detail) =>
        new(new FaultReason($"{code}: {detail}"), new FaultCode(level, new FaultCode(code)));
}
