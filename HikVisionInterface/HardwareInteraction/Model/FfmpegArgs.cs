namespace HikVisionInterface.HardwareInteraction.Model;

public class FfmpegArgs
{
    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? Format { get; set; }

    public string? InputAddr { get; set; }

    public string? UDPMulticastIp { get; set; }

    public string? UDPMulticastPort { get; set; }
}
