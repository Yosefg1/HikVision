using Serilog;

namespace HikVisionModel;

public static class SerilogLogger
{
    public static void Init()
    {
        Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();
    }

    public static void ConsoleLog(string message)
    {
        Log.Information(message);
    }

    public static void ErrorLog(string message)
    {
        Log.Error(message);
    }
}