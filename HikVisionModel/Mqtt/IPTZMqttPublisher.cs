namespace HikVisionModel.Mqtt;

/// <summary>
/// Implamentation: <see cref="PTZMqttPublisher"/>
/// additional classes at <see cref="HikVisionModel.Mqtt"/>
/// </summary>
public interface IPTZMqttPublisher
{
    bool IsConnected { get; }

    Task Connect();
    /// <summary>
    /// by defualt publishes to <see cref="PTZControl"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<bool> Publish<T>(T dto) where T : BaseDto;
    Task<bool> Publish<T>(T dto, string topic) where T : BaseDto;
}
