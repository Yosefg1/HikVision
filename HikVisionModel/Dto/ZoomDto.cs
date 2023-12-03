namespace HikVisionModel.Dto;

public class ZoomDto : BaseDto
{
    public ZoomDto(PTZControl ptz, int value)
    {
        PTZ = ptz;
        Value = value;
        DtoId = DtoEnum.ZoomDto;
    }

    public int Value { get; set; }
}