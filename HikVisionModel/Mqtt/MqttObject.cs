namespace HikVisionModel.Mqtt;

public record MqttObject
{
    public MqttObject(string? topic, string? payload)
    {
        Topic = topic;
        Payload = payload;
    }

    public string? Topic { get; set; }
    public string? Payload { get; set; }

}


public class MqttObjectEventArgs : EventArgs
{
    public MqttObject MqttObject { get; }

    public MqttObjectEventArgs(MqttObject mqttObject)
    {
        MqttObject = mqttObject;
    }
}