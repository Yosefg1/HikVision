
namespace HikVisionInterface.HardwareInteraction.Model;

public class Config
{
    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? CameraIp { get; set; }

    public string? Format { get; set; }

    public string? InputAddr { get; set; }

    public string? UDPMulticastIp { get; set; }

    public string? UDPMulticastPort { get; set; }

    public string? Format2 { get; set; }

    public string? InputAddr2 { get; set; }

    public string? UDPMulticastIp2 { get; set; }

    public string? UDPMulticastPort2 { get; set; }

    public bool ValidConfig()
    {
        return !string.IsNullOrWhiteSpace(UserName) &&
               !string.IsNullOrWhiteSpace(Password) &&
               !string.IsNullOrEmpty(CameraIp) &&
               !string.IsNullOrWhiteSpace(Format) &&
               !string.IsNullOrWhiteSpace(InputAddr) &&
               !string.IsNullOrWhiteSpace(UDPMulticastIp) &&
               !string.IsNullOrWhiteSpace(UDPMulticastPort) &&
               !string.IsNullOrWhiteSpace(Format2) &&
               !string.IsNullOrWhiteSpace(InputAddr2) &&
               !string.IsNullOrWhiteSpace(UDPMulticastIp2) &&
               !string.IsNullOrWhiteSpace(UDPMulticastPort2);
    }

    public string Print()
    {
        return $"Connecting {InputAddr} multicast available on {UDPMulticastIp}:{UDPMulticastPort}";
    }
}
