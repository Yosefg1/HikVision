using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikVisionModel.Dto;

public abstract class BaseDto
{
    public PTZControl PTZ { get; set; }

    public string ToJsonString() => JsonConvert.SerializeObject(this);

    public static T ToJsonObject<T>(string json) =>
        JsonConvert.DeserializeObject<T>(json)!;
}
