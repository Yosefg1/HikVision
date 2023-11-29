using Onvif.Core.Client.Common;
using System.Diagnostics;

namespace HikVisionInterface.HardwareInteraction;

public class FfmpegService
{
    private Config? _config;

    public FfmpegService()
    {
        _config = ConfigFactory.Build();
        _config.ValidConfig();
        if (_config is null || !_config.ValidConfig())
        {
            SerilogLogger.ErrorLog("config is not fully set up");
            return;
        }
        SerilogLogger.ConsoleLog(_config.Print());
    }

    public void StartFfmpegProcess()
    {
        Task.Run(() =>
        {
            //CloseFfmpegProcess();

            ProcessStartInfo processStartInfo = new()
            {
                UseShellExecute = false,
                CreateNoWindow = true,

                RedirectStandardInput = true,
                RedirectStandardOutput = false,
                RedirectStandardError = false,

                FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe"),
                //Arguments = "-an -i " + $"rtsp://{_config.UserName}:{_config.Password}@{_config.IP}:554" + 
                //$" -f mpegts -c copy -fflags nobuffer udp://{_config.UDPMulticastIp}:{_config.UDPMulticastPort}?pkt_size=1316"

                Arguments = "-an -i " + $"udp://@235.97.2.41:51001" +
                $" -f mpegts -c copy -fflags nobuffer udp://{_config.UDPMulticastIp}:{_config.UDPMulticastPort}?pkt_size=1316"
            };

            try
            {
                using var ffmpegProcess = new Process();

                ffmpegProcess.StartInfo = processStartInfo;
                ffmpegProcess.Start();
                if (ffmpegProcess is null) return false;
                SerilogLogger.ConsoleLog("Ffmpeg process is started.");
                return true;
            }
            catch (Exception ex)
            {
                SerilogLogger.ErrorLog(ex.Message);
                return false;
            }
        });
    }
}
