using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Configuration;
using SoapGrpcBridge.Host.Contracts;
using SoapGrpcBridge.Host.Sefaz;
using SoapGrpcBridge.Host.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddSingleton<NfeSoapService>();
builder.Services.AddSingleton<FaultTranslator>();
builder.Services.AddSingleton(serviceProvider =>
{
    var config = serviceProvider.GetRequiredService<IConfiguration>();
    var grpcUrl = config["Sefaz:GrpcUrl"]
        ?? throw new InvalidOperationException("Configuracao Sefaz:GrpcUrl ausente");
    var deadline = int.TryParse(config["Sefaz:DeadlineSeconds"], out var seconds) ? seconds : 5;
    return new SefazGrpcClient(grpcUrl, deadline);
});

var app = builder.Build();

app.MapGet("/health", () => Results.Ok("ok"));

var soapBinding = new CustomBinding(
    new TextMessageEncodingBindingElement { MessageVersion = MessageVersion.Soap12 },
    new HttpTransportBindingElement { MaxReceivedMessageSize = 1024 * 1024 });

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<NfeSoapService>(options =>
        options.BaseAddresses.Add(new Uri("http://localhost")));
    serviceBuilder.AddServiceEndpoint<NfeSoapService, INfeAuthorizationService>(soapBinding, "/nfe");
});

var metadataBehavior = app.Services.GetRequiredService<CoreWCF.Description.ServiceMetadataBehavior>();
metadataBehavior.HttpGetEnabled = true;

app.Run();

public partial class Program { }
