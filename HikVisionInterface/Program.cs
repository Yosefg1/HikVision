using HikVisionInterface.ConverterInteraction;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.ComponentModel;
using System.Net.Mime;

namespace HikVisionInterface;

public class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.UseUrls("http://127.0.0.1:5292");

        //api configuration 
        //builder.Services.AddEndpointsApiExplorer();
        //builder.Services.AddControllers();
        //builder.Services.AddAuthentication();

        builder.Services.AddSingleton<PTZMqttSubscriber>();
        builder.Services.AddSingleton<IPTZMqttPublisher, PTZMqttPublisher>();
        builder.Services.AddSingleton<MqttMessageHandler>();
        builder.Services.AddSingleton<CameraService>();
        builder.Services.AddSingleton<FfmpegManager>();
        builder.Services.AddSingleton<MuxingService>();
        builder.Services.AddSingleton<XmlUpdaterService>();

        SerilogLogger.Init();

        var app = builder.Build();

        var ffmpegManager = app.Services.GetRequiredService<FfmpegManager>();
        ffmpegManager.Initialize();

        var messageHandler = app.Services.GetRequiredService<MqttMessageHandler>();
        messageHandler.Initialize();

        app.Run();
    }

    private static ContentResult HtmlGetHandler(HttpContext ctx)
    {
        var html = System.IO.File.ReadAllText(@"./View/index.html");

        ContentResult content = new()
        {
            Content = html,
            ContentType = "text/html"
        };

        return content;
    }
}