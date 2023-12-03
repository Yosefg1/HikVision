using Newtonsoft.Json;

namespace HikVisionModel.Dto;

public abstract class BaseDto
{
    public PTZControl PTZ { get; set; }

    public DtoEnum DtoId { get; set; }

    public string ToJsonString() => JsonConvert.SerializeObject(this);

    public static T ToJsonObject<T>(string json) =>
        JsonConvert.DeserializeObject<T>(json)!;
}
