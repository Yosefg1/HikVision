using System.Diagnostics;

namespace HikVisionFfmpeg;

internal class Program
{
    static void Main(string[] args)
    {
        var config = ConfigFactory.Build();
        if (config is null || config.ValidConfig())
        {
            Console.WriteLine("config is not fully set up");
            return;
        }
        FfmpegService service = new(config);
        service.StartFfmpegProcess();

    }
}