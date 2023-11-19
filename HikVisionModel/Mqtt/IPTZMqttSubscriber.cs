using MQTTnet.Client;

namespace HikVisionModel.Mqtt;

public interface IPTZMqttSubscriber
{
    event AsyncMessageHandler? OnMessageRecived;
}
