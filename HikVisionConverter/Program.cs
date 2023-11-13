using CoreWCF.Configuration;
using HikVisionModel;

namespace HikeVisionConverter;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.UseUrls("http://127.0.0.1:5296");

        builder.Services.AddServiceModelServices();
        builder.Services.AddSingleton<MarsService>();
        builder.Services.AddSingleton<IRequestHandler, RequestHandler>();
        builder.Services.AddSingleton<ICommandHandler, CommandHandler>();
        builder.Services.AddTransient<XmlFileManager>();
        builder.Services.AddSingleton<MarsRepository>();
        builder.Services.AddSingleton<CycleTimerService>();
        builder.Services.AddSingleton<IPTZMqttPublisher, PTZMqttPublisher>();

        SerilogLogger.Init();

        var app = builder.Build();

        app.UseServiceModel(builder =>
        {
            builder.AddService<MarsService>()
            .AddServiceEndpoint<MarsService, SNSR_STDSOAPPort>(new CoreWCF.BasicHttpBinding(), "/SNSR_STD-SOAP");
        });

        app.Run();
    }
}
