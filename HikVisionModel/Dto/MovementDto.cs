using Newtonsoft.Json;

namespace HikVisionModel.Dto;

public class MovementDto : BaseDto
{
    public MovementDto(PTZControl ptz,float hVel, float vvel)
    {
        PTZ = ptz;
        HVel = hVel;
        VVel = vvel;
    }

    public float HVel { get; set; } //horizontal velocity

    public float VVel { get; set; } //vertical velocity
}
