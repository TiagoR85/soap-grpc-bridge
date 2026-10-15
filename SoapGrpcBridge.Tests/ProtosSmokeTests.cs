using Google.Protobuf;
using SoapGrpcBridge.Contracts.Sefaz.Nfe.V1;

namespace SoapGrpcBridge.Tests;

public sealed class ProtosSmokeTests
{
    [Fact]
    public void AutorizarNfeRequest_roundtrips_through_protobuf_bytes()
    {
        var original = new AutorizarNfeRequest { CUF = 35, IdLote = "1", TpAmb = 2 };
        original.Chaves.Add(new string('1', 44));

        var parsed = AutorizarNfeRequest.Parser.ParseFrom(original.ToByteArray());

        Assert.Equal(35, parsed.CUF);
        Assert.Equal("1", parsed.IdLote);
        Assert.Equal(2, parsed.TpAmb);
        Assert.Single(parsed.Chaves);
    }

    [Fact]
    public void Generated_grpc_client_type_is_available()
    {
        Assert.NotNull(typeof(NfeAuthorizationService.NfeAuthorizationServiceClient));
        Assert.Contains(
            typeof(NfeAuthorizationService.NfeAuthorizationServiceClient).GetMethods()
                .Where(method => method.Name == "AutorizarNfeAsync" && method.GetParameters().Length == 2),
            method => method.GetParameters()[1].ParameterType.Name is "CallOptions" or "CancellationToken");
    }
}
