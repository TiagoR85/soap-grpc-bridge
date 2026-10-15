using Grpc.Core;
using SoapGrpcBridge.Host.Contracts;
using SoapGrpcBridge.Host.Sefaz;

namespace SoapGrpcBridge.Tests.Sefaz;

public sealed class FaultTranslatorTests
{
    [Fact]
    public void Unavailable_maps_to_backend_unavailable_server_fault()
    {
        var exception = new RpcException(new Status(StatusCode.Unavailable, "connection refused"));

        var fault = FaultTranslator.ToFault(exception);

        Assert.Equal("Server", fault.Code.Name);
        Assert.Equal(SefazFault.BackendUnavailableCode, fault.Code.SubCode?.Name);
        Assert.Contains(SefazFault.BackendUnavailableCode, fault.Reason.ToString());
    }

    [Fact]
    public void DeadlineExceeded_maps_to_timeout_server_fault()
    {
        var exception = new RpcException(new Status(StatusCode.DeadlineExceeded, "deadline exceeded"));

        var fault = FaultTranslator.ToFault(exception);

        Assert.Equal("Server", fault.Code.Name);
        Assert.Equal(SefazFault.TimeoutCode, fault.Code.SubCode?.Name);
    }

    [Fact]
    public void Any_other_status_maps_to_backend_unavailable()
    {
        var exception = new RpcException(new Status(StatusCode.Internal, "boom"));

        var fault = FaultTranslator.ToFault(exception);

        Assert.Equal(SefazFault.BackendUnavailableCode, fault.Code.SubCode?.Name);
    }

    [Fact]
    public void InvalidRequest_fault_is_client_level_with_stable_code()
    {
        var fault = SefazFault.InvalidRequest("lote sem chaves");

        Assert.Equal("Client", fault.Code.Name);
        Assert.Equal(SefazFault.InvalidRequestCode, fault.Code.SubCode?.Name);
        Assert.Contains(SefazFault.InvalidRequestCode, fault.Reason.ToString());
    }
}
