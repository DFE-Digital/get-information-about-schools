
using Microsoft.Extensions.Logging;

public sealed class ReqnrollLogger : IAcceptanceTestLogger
{
    private readonly IReqnrollOutputHelper reqnrollOutputHelper;

    public ReqnrollLogger(IReqnrollOutputHelper reqnrollOutputHelper)
    {
        this.reqnrollOutputHelper = reqnrollOutputHelper;
    }

    public void LogError(string message)
    {
        Log(LogLevel.Error, message);
    }

    private void Log(LogLevel logLevel, string message)
    {
        reqnrollOutputHelper.WriteLine($"[{DateTime.Now} {logLevel}] {message}");
    }
}
