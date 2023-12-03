using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikVisionModel.Dto;

public class PanTiltDto : BaseDto
{
    public PanTiltDto(string tilt,
        string pan)
    {
        Tilt = tilt;
        Pan = pan;
        PTZ = PTZControl.Status;
        DtoId = DtoEnum.PanTiltDto;
    }

    public string Pan { get; set; }
    public string Tilt { get; set; }
}
