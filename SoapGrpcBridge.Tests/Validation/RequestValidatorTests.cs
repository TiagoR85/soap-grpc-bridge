using SoapGrpcBridge.Host.Contracts;
using SoapGrpcBridge.Host.Validation;

namespace SoapGrpcBridge.Tests.Validation;

public sealed class RequestValidatorTests
{
    [Fact]
    public void Empty_lote_is_rejected()
    {
        var dto = new AutorizarNfeRequest { cUF = 35, idLote = "1", tpAmb = 2, chaves = [] };

        Assert.NotNull(RequestValidator.Validate(dto));
    }

    [Fact]
    public void Null_chaves_is_rejected()
    {
        var dto = new AutorizarNfeRequest { cUF = 35, idLote = "1", tpAmb = 2, chaves = null! };

        Assert.NotNull(RequestValidator.Validate(dto));
    }

    [Fact]
    public void OutOfRange_cUF_is_rejected()
    {
        var dto = new AutorizarNfeRequest { cUF = 99, idLote = "1", tpAmb = 2, chaves = ["1"] };

        Assert.NotNull(RequestValidator.Validate(dto));
    }

    [Fact]
    public void Gap_cUF_is_rejected()
    {
        var dto = new AutorizarNfeRequest { cUF = 18, idLote = "1", tpAmb = 2, chaves = ["1"] };

        Assert.NotNull(RequestValidator.Validate(dto));
    }

    [Fact]
    public void Empty_idLote_is_rejected()
    {
        var dto = new AutorizarNfeRequest { cUF = 35, idLote = "  ", tpAmb = 2, chaves = ["1"] };

        Assert.NotNull(RequestValidator.Validate(dto));
    }

    [Fact]
    public void Valid_authorization_request_passes()
    {
        var dto = new AutorizarNfeRequest { cUF = 35, idLote = "1", tpAmb = 2, chaves = [new string('1', 44)] };

        Assert.Null(RequestValidator.Validate(dto));
    }

    [Fact]
    public void Valid_status_request_passes()
    {
        Assert.Null(RequestValidator.Validate(new ConsultarStatusServicoRequest { cUF = 35, tpAmb = 2 }));
    }

    [Fact]
    public void Invalid_status_cUF_is_rejected()
    {
        Assert.NotNull(RequestValidator.Validate(new ConsultarStatusServicoRequest { cUF = 99, tpAmb = 2 }));
    }
}
