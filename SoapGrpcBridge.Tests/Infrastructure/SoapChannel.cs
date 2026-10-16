using System.ServiceModel;
using System.ServiceModel.Channels;

namespace SoapGrpcBridge.Tests.Infrastructure;

internal static class SoapChannel
{
    public static async Task<T> CallAsync<T>(Uri endpoint, Func<INfeAuthorizationChannel, Task<T>> call)
    {
        var binding = new CustomBinding(
            new TextMessageEncodingBindingElement { MessageVersion = MessageVersion.Soap12 },
            new HttpTransportBindingElement { MaxReceivedMessageSize = 1024 * 1024 });

        var factory = new ChannelFactory<INfeAuthorizationChannel>(binding, new EndpointAddress(endpoint));
        var channel = factory.CreateChannel();
        try
        {
            return await call(channel);
        }
        finally
        {
            ((ICommunicationObject)channel).Abort();
            factory.Abort();
        }
    }
}
