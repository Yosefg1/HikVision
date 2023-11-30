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
        //builder.Services.AddSingleton<FfmpegService>();
        builder.Services.AddSingleton<XmlUpdaterService>();

        SerilogLogger.Init();

        var app = builder.Build();

        var config = ConfigFactory.Build();

        var ffmpeg = FfmpegServiceFactory.Build(config, 1);
        var ffmpeg2 = FfmpegServiceFactory.Build(config, 2);

        ffmpeg.StartFfmpegProcess();
        ffmpeg2.StartFfmpegProcess();

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