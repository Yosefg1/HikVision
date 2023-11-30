using System.Diagnostics;

namespace HikVisionInterface.HardwareInteraction;

public class FfmpegService
{
    readonly FfmpegArgs? _config;

    public FfmpegService(FfmpegArgs? config)
    {
        _config = config;
    }

    public void StartFfmpegProcess(bool WithAuth = true)
    {
        Task.Run(() =>
        {
            string Auth = string.Empty;

            if (WithAuth && _config!.UserName != "" && _config.Password != string.Empty)
                Auth = $"{_config!.UserName}:{_config.Password}";

            //CloseFfmpegProcess();

            ProcessStartInfo processStartInfo = new()
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                //CreateNoWindow = false,
                //WindowStyle = ProcessWindowStyle.Normal,

                RedirectStandardInput = true,
                RedirectStandardOutput = false,
                RedirectStandardError = false,

                FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe"),

                Arguments = "-an -i " + $"{_config!.Format}://{Auth}@{_config.InputAddr}" +
                $" -f mpegts -c copy -fflags nobuffer udp://{_config.UDPMulticastIp}:{_config.UDPMulticastPort}?pkt_size=1316"
            };

            try
            {
                using var ffmpegProcess = new Process();

                ffmpegProcess.StartInfo = processStartInfo;
                ffmpegProcess.Start();
                if (ffmpegProcess is null) return false;
                SerilogLogger.ConsoleLog($"ffmpeg process is started. broadcasting to udp://{_config.UDPMulticastIp}:{_config.UDPMulticastPort}");
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
