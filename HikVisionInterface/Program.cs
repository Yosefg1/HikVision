using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Net.Mime;

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



        builder.Services.AddSingleton<IPTZMqttSubscriber, PTZMqttSubscriber>();
        builder.Services.AddSingleton<MqttMessageHandler>();
        builder.Services.AddSingleton<CameraService>();

        SerilogLogger.Init();

        var app = builder.Build();

        app.MapGet("", (HttpContext httpContext) =>
        {
            var html = System.IO.File.ReadAllText(@"./View/index.html");

            return new ContentResult();
        });
        app.MapPost("save", (HttpContext httpContext) =>
        {
            var config = httpContext.Request.Body;

            return httpContext.Response;
        });

        var mqtt = app.Services.GetRequiredService<IPTZMqttSubscriber>();
        mqtt.Connect();

        app.Run();
    }
}