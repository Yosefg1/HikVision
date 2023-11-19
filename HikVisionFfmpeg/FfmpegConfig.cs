using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikVisionFfmpeg;

public record FfmpegConfig
{
    
    public string? UserName { get; private set; }

    public string? Password { get; private set; }

    public string? CameraIp { get; private set; }

    public string? UDPMulticastIp { get; private set; }

    public string? UDPMulticastPort { get; private set; }

    public bool ValidConfig ()
    {
        return !string.IsNullOrWhiteSpace(UserName) &&
               !string.IsNullOrWhiteSpace(Password) &&
               !string.IsNullOrWhiteSpace(CameraIp) &&
               !string.IsNullOrWhiteSpace(UDPMulticastIp) &&
               !string.IsNullOrWhiteSpace(UDPMulticastIp);
    }
}
