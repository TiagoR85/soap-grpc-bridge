using Grpc.Core;
using SoapGrpcBridge.Host.Contracts;
using SoapGrpcBridge.Host.Mapping;
using SoapGrpcBridge.Host.Sefaz;
using SoapGrpcBridge.Host.Validation;

namespace SoapGrpcBridge.Host.Services;

public sealed class NfeSoapService : INfeAuthorizationService
{
    private readonly SefazGrpcClient _sefaz;
    private readonly ILogger<NfeSoapService> _logger;

    public NfeSoapService(SefazGrpcClient sefaz, ILogger<NfeSoapService> logger)
    {
        _sefaz = sefaz;
        _logger = logger;
    }

    public async Task<AutorizarNfeResponse> AutorizarNfe(AutorizarNfeRequest request)
    {
        var error = RequestValidator.Validate(request);
        if (error is not null) throw SefazFault.InvalidRequest(error);

        try
        {
            var protoResponse = await _sefaz.AutorizarNfeAsync(NfeMapper.ToProto(request));

            if (protoResponse.CStat == 100 &&
                (string.IsNullOrEmpty(protoResponse.NRec) || string.IsNullOrEmpty(protoResponse.NProt)))
            {
                throw SefazFault.IntegrityError("resposta cStat 100 sem nRec/nProt");
            }

            var response = NfeMapper.ToResponse(protoResponse);
            _logger.LogInformation("AutorizarNfe cStat={CStat} nRec={NRec}", response.cStat, response.nRec);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogWarning(ex, "Backend gRPC falhou em AutorizarNfe");
            throw FaultTranslator.ToFault(ex);
        }
    }

    public async Task<ConsultarStatusServicoResponse> ConsultarStatusServico(ConsultarStatusServicoRequest request)
    {
        var error = RequestValidator.Validate(request);
        if (error is not null) throw SefazFault.InvalidRequest(error);

        try
        {
            var protoResponse = await _sefaz.ConsultarStatusServicoAsync(NfeMapper.ToProto(request));
            var response = NfeMapper.ToResponse(protoResponse);
            _logger.LogInformation("ConsultarStatusServico cStat={CStat}", response.cStat);
            return response;
        }
        catch (RpcException ex)
        {
            _logger.LogWarning(ex, "Backend gRPC falhou em ConsultarStatusServico");
            throw FaultTranslator.ToFault(ex);
        }
    }
}
