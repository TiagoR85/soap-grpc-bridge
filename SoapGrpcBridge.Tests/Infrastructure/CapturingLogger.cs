using System.Collections.Concurrent;
using WireMock.Admin.Requests;
using WireMock.Logging;

namespace SoapGrpcBridge.Tests.Infrastructure;

internal sealed class CapturingLogger : IWireMockLogger
{
    public static readonly CapturingLogger Instance = new();

    private readonly ConcurrentQueue<string> _messages = new();

    public IReadOnlyCollection<string> Messages => _messages.ToArray();

    public void Clear() => _messages.Clear();

    public void Info(string message, params object[] args) => _messages.Enqueue($"[Info] {message}");

    public void Debug(string message, params object[] args)
    {
        if (message.Contains("Exception", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("error", StringComparison.OrdinalIgnoreCase))
        {
            _messages.Enqueue($"[Debug] {message}");
        }
    }

    public void Warn(string message, params object[] args) => _messages.Enqueue($"[Warn] {message}");

    public void Error(string message, params object[] args) => _messages.Enqueue($"[Error] {message}");

    public void Error(string message, Exception exception) => _messages.Enqueue($"[Error] {message} :: {exception}");

    public void DebugRequestResponse(LogEntryModel logEntryModel, bool isProxy) { }
}
