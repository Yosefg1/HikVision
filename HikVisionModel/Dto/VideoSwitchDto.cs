﻿namespace HikVisionModel.Dto;

public class VideoSwitchDto : BaseDto
{
    public VideoSwitchDto(PTZControl ptz)
    {
        PTZ = ptz;
        VideoSwitch = PTZ is PTZControl.DayMode;
        DtoId = DtoEnum.VideoSwitchDto;
    }

    /// <summary>
    /// true of daycamera false if bw camera
    /// </summary>
    public bool VideoSwitch { get; private set; }
}