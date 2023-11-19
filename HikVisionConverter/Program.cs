using CoreWCF.Configuration;

namespace HikeVisionConverter;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var ip = IP.GetLocalIPAddress();
        builder.WebHost.UseUrls("http://*:5296");

        builder.Services.AddServiceModelServices();
        builder.Services.AddSingleton<MarsService>();
        builder.Services.AddSingleton<IRequestHandler, RequestHandler>();
        builder.Services.AddSingleton<ICommandHandler, CommandHandler>();
        builder.Services.AddTransient<XmlFileManager>();
        builder.Services.AddSingleton<MarsRepository>();
        builder.Services.AddSingleton<CycleTimerService>();

        builder.Services.AddSingleton<IPTZMqttPublisher, PTZMqttPublisher>();
        builder.Services.AddSingleton<PTZMqttSubscriber>();


        SerilogLogger.Init();
        SerilogLogger.ConsoleLog($"Running on http://{ip}:5296");

        var app = builder.Build();


        app.UseServiceModel(builder =>
        {
            builder.AddService<MarsService>()
            .AddServiceEndpoint<MarsService, SNSR_STDSOAPPort>(new CoreWCF.BasicHttpBinding(), "/SNSR_STD-SOAP");
        });

        app.Run();
    }
}
