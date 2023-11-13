using Serilog;

namespace HikVisionInterface.HardwareInteraction;

public class MqttMessageHandler
{
    private readonly CameraService _camera;

    public MqttMessageHandler(CameraService cameraService)
    {
        _camera = cameraService;
    }

    public async Task HandleMqttMessage(string topic, string payload)
    {
        // var dto = MovementDto.ToJsonObject(topic);
        List<string> enumList = Enum.GetValues(typeof(PTZControl))
                                .Cast<PTZControl>()
                                .Select(x => x.ToString())
                                .ToList();

        var dto = MovementDto.ToJsonObject(payload);

        if (!enumList.Contains(dto.PTZ.ToString())) return;

        PTZControl eTopic = (PTZControl)Enum.Parse(typeof(PTZControl), dto.PTZ.ToString());

        //Log.Information($"{eTopic}");
        //Log.Information($"{topic}.{dto.ToJsonString()}");

        await _camera.MoveAsync(dto);
    }
}
