
using HikVisionApiInterface.ConverterInteraction;
using Serilog;

namespace HikVisionApiInterface;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.UseUrls("http://127.0.0.1:5292");

        //builder.Services.AddSingleton<CameraClient>();
        builder.Services.AddSingleton<IPTZMqttSubscriber, PTZMqttSubscriber>();
        builder.Services.AddSingleton<MqttMessageHandler>();

        Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();

        var app = builder.Build();

        var mqtt = app.Services.GetRequiredService<IPTZMqttSubscriber>();
        mqtt.Connect();

        app.Run();
    }
}