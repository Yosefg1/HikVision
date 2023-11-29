namespace HikVisionModel.Dto;

public class VideoSwitchDto : BaseDto
{
    public VideoSwitchDto(PTZControl ptz)
    {
        PTZ = ptz;
        VideoSwitch = PTZ is PTZControl.DayMode ? 1 : 2;
    }

    public int VideoSwitch { get; set; }
}
