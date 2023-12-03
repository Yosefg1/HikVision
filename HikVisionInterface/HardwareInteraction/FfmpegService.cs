using HikVisionModel;
using System.Diagnostics;

namespace HikVisionInterface.HardwareInteraction;

public class FfmpegService
{
    public FfmpegArgs? Config { get; private set; }
    private Process? _ffmpegProcess;

    public FfmpegService(FfmpegArgs? config)
    {
        Config = config;
    }

    /// <summary>
    /// Also closes previous Ffmpeg process
    /// </summary>
    public void ChangeInput(string inputAddress)
    {
        if(Config is null || inputAddress == string.Empty)
        {
            SerilogLogger.ErrorLog("Configuration is null");
            return;
        }

        Stop();

        Config!.InputAddr = inputAddress;
    }

    /// <summary>
    /// contains inner exception handling
    /// </summary>
    /// <param name="withAuth">bool by default</param>
    public void Start(bool withAuth = true)
    {
        Task.Run(() =>
        {
            string auth = string.Empty;

            if (withAuth && !string.IsNullOrEmpty(Config?.UserName) && !string.IsNullOrEmpty(Config?.Password))
                auth = $"{Config!.UserName}:{Config.Password}";

            ProcessStartInfo processStartInfo = new()
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe"),
                Arguments = "-an -i " + $"{Config!.Format}://{auth}@{Config.InputAddr}" +
                            $" -f mpegts -c copy -fflags nobuffer udp://{Config.UDPMulticastIp}:{Config.UDPMulticastPort}?pkt_size=1316"
            };

            try
            {
                _ffmpegProcess = new Process
                {
                    StartInfo = processStartInfo
                };
                _ffmpegProcess.Start();
                if (_ffmpegProcess == null) return;
                SerilogLogger.ConsoleLog($"ffmpeg process is started. Broadcasting to udp://{Config.UDPMulticastIp}:{Config.UDPMulticastPort}");
            }
            catch (Exception ex)
            {
                SerilogLogger.ErrorLog(ex.Message);
            }
        });
    }
    

    
    public async Task StartAsync(bool withAuth = true)
    {
        await Task.Run(() =>
        {
            string auth = string.Empty;

            if (withAuth && !string.IsNullOrEmpty(Config?.UserName) && !string.IsNullOrEmpty(Config?.Password))
                auth = $"{Config!.UserName}:{Config.Password}";

            ProcessStartInfo processStartInfo = new()
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe"),
                Arguments = "-an -i " + $"{Config!.Format}://{auth}@{Config.InputAddr}" +
                            $" -f mpegts -c copy -fflags nobuffer udp://{Config.UDPMulticastIp}:{Config.UDPMulticastPort}?pkt_size=1316"
            };

            try
            {
                _ffmpegProcess = new Process
                {
                    StartInfo = processStartInfo
                };
                _ffmpegProcess.Start();
                if (_ffmpegProcess == null) return;
                SerilogLogger.ConsoleLog($"ffmpeg process is started. Broadcasting to udp://{Config.UDPMulticastIp}:{Config.UDPMulticastPort}");
            }
            catch (Exception ex)
            {
                SerilogLogger.ErrorLog(ex.Message);
            }
        });
    }

    /// <summary>
    /// With inner exception handling disposes and kills the process
    /// </summary>
    public void Stop()
    {
        try
        {
            _ffmpegProcess?.Kill();
            _ffmpegProcess?.Dispose();
            _ffmpegProcess = null;
            SerilogLogger.ConsoleLog("ffmpeg process is stopped.");
        }
        catch (Exception ex)
        {
            SerilogLogger.ErrorLog(ex.Message);
        }
    }
}
