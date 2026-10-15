using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Proto = SoapGrpcBridge.Contracts.Sefaz.Nfe.V1;

namespace SoapGrpcBridge.Tests.Infrastructure;

internal static class SefazStubs
{
    public const string AutorizarPath = "/sefaz.nfe.v1.NfeAuthorizationService/AutorizarNfe";
    public const string StatusPath = "/sefaz.nfe.v1.NfeAuthorizationService/ConsultarStatusServico";

    public static byte[] GrpcFrame(IMessage message)
    {
        var payload = message.ToByteArray();
        var frame = new byte[5 + payload.Length];
        frame[0] = 0;
        System.Buffers.Binary.BinaryPrimitives.WriteInt32BigEndian(frame.AsSpan(1), payload.Length);
        payload.CopyTo(frame, 5);
        return frame;
    }

    private static IResponseBuilder GrpcResponse(IResponseBuilder response, byte[] frame) =>
        response
            .WithHeader("Content-Type", "application/grpc")
            .WithTrailingHeader("grpc-status", "0")
            .WithBody(frame, null, null);

    public static void StubAutorizar(
        SefazStubFixture fixture,
        int cStat,
        string nRec,
        string nProt,
        string xMotivo,
        int delayMilliseconds = 0)
    {
        var payload = new Proto.AutorizarNfeResponse { CStat = cStat, NRec = nRec, NProt = nProt, XMotivo = xMotivo };
        var response = GrpcResponse(Response.Create(), GrpcFrame(payload));

        if (delayMilliseconds > 0)
        {
            response = response.WithDelay(delayMilliseconds);
        }

        fixture.Server
            .Given(Request.Create()
                .UsingPost()
                .WithPath(AutorizarPath))
            .RespondWith(response);
    }

    public static void StubStatus(
        SefazStubFixture fixture,
        int cStat,
        string xMotivo,
        string dhRecbto,
        string versao,
        int delayMilliseconds = 0)
    {
        var payload = new Proto.ConsultarStatusServicoResponse
        {
            CStat = cStat, XMotivo = xMotivo, DhRecbto = dhRecbto, Versao = versao
        };
        var response = GrpcResponse(Response.Create(), GrpcFrame(payload));

        if (delayMilliseconds > 0)
        {
            response = response.WithDelay(delayMilliseconds);
        }

        fixture.Server
            .Given(Request.Create()
                .UsingPost()
                .WithPath(StatusPath))
            .RespondWith(response);
    }
}
