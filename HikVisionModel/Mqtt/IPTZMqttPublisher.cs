namespace HikVisionModel.Mqtt;

public interface IPTZMqttPublisher
{
    bool IsConnected { get; }

    Task Connect();
    Task<bool> Publish(PTZControl PTZ);
    Task<bool> Publish<T>(T dto) where T : BaseDto;
    Task<bool> Publish<T>(T dto, string topic) where T : BaseDto;
}
