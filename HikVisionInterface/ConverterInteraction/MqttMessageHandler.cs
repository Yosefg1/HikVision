using HikVisionModel.Enums;
using Serilog;
using System.Text;

namespace HikVisionInterface.ConverterInteraction;

public class MqttMessageHandler
{
    private readonly MuxingService _muxer;
    private readonly PTZMqttSubscriber _subscriber;

    public MqttMessageHandler(MuxingService muxer,
        PTZMqttSubscriber subscriber)
    {
        _subscriber = subscriber;
        _muxer = muxer;
    }

    public void Initialize()
    {
        _subscriber.OnMessageRecived += Mqtt_OnMessageRecived;

        _subscriber.Subscribe(nameof(PTZControl));
        _subscriber.Subscribe(nameof(Topics.VideoSwitch));
    }

    private async Task Mqtt_OnMessageRecived(object? sender, MqttObject e)
    {
        await _muxer.Mux(e.Payload!, e.Topic!);
    }
}
