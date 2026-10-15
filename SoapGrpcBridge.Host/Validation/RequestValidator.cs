using SoapGrpcBridge.Host.Contracts;

namespace SoapGrpcBridge.Host.Validation;

public static class RequestValidator
{
    private static readonly HashSet<int> CodigosUf =
    [
        11, 12, 13, 14, 15, 16, 17, 21, 22, 23, 24, 25, 26, 27, 28, 29,
        31, 32, 33, 35, 41, 42, 43, 50, 51, 52, 53
    ];

    public static string? Validate(AutorizarNfeRequest? request)
    {
        if (request is null) return "pedido nulo";
        if (!CodigosUf.Contains(request.cUF)) return $"cUF invalido: {request.cUF}";
        if (string.IsNullOrWhiteSpace(request.idLote)) return "idLote vazio";
        if (request.chaves is null || request.chaves.Length == 0) return "lote sem chaves";
        return null;
    }

    public static string? Validate(ConsultarStatusServicoRequest? request)
    {
        if (request is null) return "pedido nulo";
        if (!CodigosUf.Contains(request.cUF)) return $"cUF invalido: {request.cUF}";
        return null;
    }
}
