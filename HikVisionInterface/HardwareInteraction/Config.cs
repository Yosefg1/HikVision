
namespace HikVisionApiInterface.HardwareInteraction;

public class Config
{
    public string IP { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }

    public bool ValidConfig()
    {
        if(this is null) return false;
        return !string.IsNullOrWhiteSpace(UserName) &&
               !string.IsNullOrWhiteSpace(Password) &&
               !string.IsNullOrWhiteSpace(IP);
    }
}
