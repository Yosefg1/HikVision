using Microsoft.AspNetCore.Mvc;


namespace HikVisionInterface;

public class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.UseUrls("http://127.0.0.1:5292");

        //api configuration 
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddControllers();
        builder.Services.AddAuthentication();

        builder.Services.AddSingleton<PTZMqttSubscriber>();
        builder.Services.AddSingleton<IPTZMqttPublisher, PTZMqttPublisher>();
        builder.Services.AddSingleton<MqttMessageHandler>();
        builder.Services.AddSingleton<CameraService>();
        builder.Services.AddSingleton<FfmpegService>();
        builder.Services.AddSingleton<XmlUpdaterService>();

        SerilogLogger.Init();

        var app = builder.Build();


        var messageHandler = app.Services.GetRequiredService<MqttMessageHandler>();
        messageHandler.Initialize();
        var ffmpeg = app.Services.GetRequiredService<FfmpegService>();
        ffmpeg.StartFfmpegProcess();

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