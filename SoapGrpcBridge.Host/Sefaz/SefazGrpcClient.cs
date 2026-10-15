using Grpc.Net.Client;
using Proto = SoapGrpcBridge.Contracts.Sefaz.Nfe.V1;

namespace SoapGrpcBridge.Host.Sefaz;

public sealed class SefazGrpcClient : IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly Proto.NfeAuthorizationService.NfeAuthorizationServiceClient _client;
    private readonly int _deadlineSeconds;

    public SefazGrpcClient(string grpcUrl, int deadlineSeconds)
    {
        _channel = GrpcChannel.ForAddress(grpcUrl);
        _client = new Proto.NfeAuthorizationService.NfeAuthorizationServiceClient(_channel);
        _deadlineSeconds = deadlineSeconds;
    }

    public Task<Proto.AutorizarNfeResponse> AutorizarNfeAsync(Proto.AutorizarNfeRequest request, CancellationToken cancellationToken = default) =>
        _client.AutorizarNfeAsync(request, deadline: DateTime.UtcNow.AddSeconds(_deadlineSeconds), cancellationToken: cancellationToken).ResponseAsync;

    public Task<Proto.ConsultarStatusServicoResponse> ConsultarStatusServicoAsync(Proto.ConsultarStatusServicoRequest request, CancellationToken cancellationToken = default) =>
        _client.ConsultarStatusServicoAsync(request, deadline: DateTime.UtcNow.AddSeconds(_deadlineSeconds), cancellationToken: cancellationToken).ResponseAsync;

    public void Dispose() => _channel.Dispose();
}
