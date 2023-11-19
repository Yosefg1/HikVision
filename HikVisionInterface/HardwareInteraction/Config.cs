
namespace HikVisionInterface.HardwareInteraction;

public class Config
{
    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? IP { get; set; }

    public string? UDPMulticastIp { get; set; }

    public string? UDPMulticastPort { get; set; }

    public bool ValidConfig()
    {
        return !string.IsNullOrWhiteSpace(UserName) &&
               !string.IsNullOrWhiteSpace(Password) &&
               !string.IsNullOrWhiteSpace(IP) &&
               !string.IsNullOrWhiteSpace(UDPMulticastIp) &&
               !string.IsNullOrWhiteSpace(UDPMulticastPort);
    }

    public string Print()
    {
        return $"Connecting {IP} multicast available on {UDPMulticastIp}:{UDPMulticastPort}";
    }
}
