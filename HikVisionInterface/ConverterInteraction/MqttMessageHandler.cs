using Serilog;
using System.Text;

namespace HikVisionInterface.ConverterInteraction;

public class MqttMessageHandler
{
    private readonly CameraService _camera;
    private readonly PTZMqttSubscriber _subscriber;

    public MqttMessageHandler(CameraService cameraService,
        PTZMqttSubscriber subscriber,
        XmlUpdaterService xml)
    {
        _camera = cameraService;
        _subscriber = subscriber;
        xml.Start();
    }

    public void Initialize()
    {
        _subscriber.OnMessageRecived += Mqtt_OnMessageRecived;
        _subscriber.Subscribe(nameof(PTZControl));
    }

    private async Task Mqtt_OnMessageRecived(object? sender, MqttObject e)
    {
        await HandleMqttMessage(e.Topic!, e.Payload!);
    }

    public async Task HandleMqttMessage(string topic, string payload)
    {
        // var dto = MovementDto.ToJsonObject(topic);
        List<string> enumList = Enum.GetValues(typeof(PTZControl))
                                .Cast<PTZControl>()
                                .Select(x => x.ToString())
                                .ToList();

        var dto = MovementDto.ToJsonObject<MovementDto>(payload);

        if (!enumList.Contains(dto.PTZ.ToString())) return;

        PTZControl eTopic = (PTZControl)Enum.Parse(typeof(PTZControl), dto.PTZ.ToString());

        //Log.Information($"{eTopic}");
        //Log.Information($"{topic}.{dto.ToJsonString()}");
        SerilogLogger.ConsoleLog($"{topic}.{dto.PTZ} " + $"(x:{dto.HVel},y:{dto.VVel})");

        await _camera.MoveAsync(dto);
    }
}
