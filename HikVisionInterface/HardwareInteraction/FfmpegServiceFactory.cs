namespace HikVisionInterface.HardwareInteraction;

internal class FfmpegServiceFactory
{
    /// <summary>
    /// 2 for second ffmpeg service
    /// </summary>
    /// <param name="config"></param>
    /// <param name="channel"></param>
    /// <returns></returns>
    public static FfmpegService Build(Config config, int channel = 1)
    {
        FfmpegArgs args = new()
        {
            UserName = config.UserName,
            Password = config.Password
        };

        if (channel == 2)
        {
            args.Format = config.Format2;
            args.UDPMulticastIp = config.UDPMulticastIp2;
            args.UDPMulticastPort = config.UDPMulticastPort2;
            args.InputAddr = config.InputAddr2;
        }
        else
        {
            args.Format = config.Format;
            args.UDPMulticastIp = config.UDPMulticastIp;
            args.UDPMulticastPort = config.UDPMulticastPort;
            args.InputAddr = config.InputAddr;
        }


        return new FfmpegService(args);
    }
}
