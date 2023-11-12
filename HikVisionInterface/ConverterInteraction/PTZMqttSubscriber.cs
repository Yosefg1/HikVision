using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using System.Text;

namespace HikVisionApiInterface.ConverterInteraction;

public interface IPTZMqttSubscriber
{
    void Connect();
}

public class PTZMqttSubscriber : IPTZMqttSubscriber
{
    private IMqttClient _mqttClient;
    private MqttClientOptions _options;
    private readonly MqttMessageHandler _handler;

    public PTZMqttSubscriber(MqttMessageHandler handler)
    {
        _handler = handler;

        MqttFactory factory = new();
        _mqttClient = factory.CreateMqttClient();
        _options = new MqttClientOptionsBuilder()
            .WithClientId(Guid.NewGuid().ToString())
            .WithTcpServer("127.0.0.1")
            .WithCleanSession()
            .Build();

        _mqttClient.ApplicationMessageReceivedAsync += ApplicationMessageReceivedAsync;
    }

    private async Task ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        var payload = Encoding.UTF8.GetString(arg.ApplicationMessage.PayloadSegment);
        var topic = arg.ApplicationMessage.Topic;

        await _handler.HandleMqttMessage(topic, payload);
    }

    public void Connect()
    {

        _mqttClient.ConnectAsync(_options).Wait();

        Subscribe(nameof(PTZControl));

    }

    private void Subscribe(string topic)
    {
        var topicFilter = new MqttTopicFilterBuilder()
        .WithTopic(topic)
        .Build();

        _mqttClient.SubscribeAsync(topicFilter).Wait();
    }
}
