using Onvif.Core.Client;
using System.Timers;

namespace HikVisionInterface.ConverterInteraction;

public class XmlUpdaterService
{
    private readonly IPTZMqttPublisher _publisher;
    private readonly CameraService _camera;

    public XmlUpdaterService(IPTZMqttPublisher publisher,
        CameraService camera)
    {
        _publisher = publisher;
        _camera = camera;
    }

    public void Start()
    {
        _publisher.Connect();
    }

    private void UpdatePanTilt(object? sender, ElapsedEventArgs e)
    {
        var res = _camera.GetPanTiltAsync();
        
        var pan = res.GetAwaiter().GetResult().Pan;
        var tilt = res.GetAwaiter().GetResult().Tilt;
        PanTiltDto dto = new(pan, tilt);

        _publisher.Publish(dto, nameof(Topics.PanTiltStatus));
    }
}
