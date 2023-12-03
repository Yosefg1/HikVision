namespace HikVisionInterface.HardwareInteraction;

public class FfmpegManager
{
    public FfmpegService? Ffmpeg1 { get; private set; }

    public FfmpegService? Ffmpeg2 { get; private set; }

    /// <summary>
    /// if value is true it means that daycamera is on secondary...
    /// </summary>
    public bool IsSwitch { get; private set; }

    private Config? _config;

    public void Initialize()
    {
        _config = ConfigFactory.Build();
        if (_config is null)
        {
            SerilogLogger.ErrorLog("Null configuration..., Might be wrong format");
            return;
        }

        this.Ffmpeg1 = FfmpegServiceFactory.Build(_config, 1);

        this.Ffmpeg2 = FfmpegServiceFactory.Build(_config, 2);

        IsSwitch = false;

        Ffmpeg1.Start();
        Ffmpeg2.Start();
    }

    public void Switch()
    {
        var inp1 = Ffmpeg1!.Config!.InputAddr;
        var inp2 = Ffmpeg2!.Config!.InputAddr;

        SerilogLogger.ConsoleLog("Switching Multicasts...");
        Ffmpeg1.ChangeInput(inp2!);
        Ffmpeg2.ChangeInput(inp1!);

        IsSwitch = !IsSwitch;

        Ffmpeg1.Start();
        Ffmpeg2.Start();

    }
}
