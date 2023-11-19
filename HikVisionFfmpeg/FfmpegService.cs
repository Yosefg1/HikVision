using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikVisionFfmpeg;

public class FfmpegService
{
    private readonly FfmpegConfig _config;
    private Process? _ffmpegProcess;

    public FfmpegService(FfmpegConfig config)
    {
        _config = config;
    }

    public bool StartFfmpegProcess()
    {
        CloseFfmpegProcess();

        ProcessStartInfo processStartInfo = new()
        {
            UseShellExecute = false,
            CreateNoWindow = true,

            RedirectStandardInput = true,
            RedirectStandardOutput = false,
            RedirectStandardError = false,

            FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe"),
            Arguments = "-an -i " + $"rtsp://{_config.UserName}:{_config.Password}@{_config.CameraIp}/defaultPrimary" + " -f mpegts -c copy -fflags nobuffer udp://"
                            + _config.UDPMulticastIp + ":" + _config.UDPMulticastPort+ "?pkt_size=1316"
        };

        try
        {
            _ffmpegProcess = Process.Start(processStartInfo)!;
            if (_ffmpegProcess is null) return false;
            Console.WriteLine("Ffmpeg process is started.");
            return true;
        }
        catch (Exception ex) 
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    private void CloseFfmpegProcess()
    {
        if (_ffmpegProcess == null)
            return;

        try
        {
            _ffmpegProcess.StandardInput.Write('q');
            _ffmpegProcess.StandardInput.Close();
            _ffmpegProcess.CloseMainWindow();
            _ffmpegProcess.Kill();
            _ffmpegProcess.Close();
            _ffmpegProcess.Dispose();
        }
        catch
        {
            // Ignored
        }

        _ffmpegProcess = null;
    }
}
