using MQTTnet.Client;
using MQTTnet;
using System.Text;

namespace HikVisionModel.Mqtt;

public class PTZMqttSubscriber : IPTZMqttSubscriber
{
    private MqttClientOptions _options;

    private IMqttClient _mqttClient;

    public event AsyncMessageHandler? OnMessageRecived;

    public PTZMqttSubscriber()
    {
        MqttFactory factory = new();
        _mqttClient = factory.CreateMqttClient();
        _options = new MqttClientOptionsBuilder()
            .WithClientId(Guid.NewGuid().ToString())
            .WithTcpServer("127.0.0.1")
            .WithCleanSession()
            .Build();

        _mqttClient.ApplicationMessageReceivedAsync += ApplicationMessageReceivedAsync;
        Connect();
    }

    private async Task ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        var payload = Encoding.UTF8.GetString(arg.ApplicationMessage.PayloadSegment);
        var topic = arg.ApplicationMessage.Topic;

        if (OnMessageRecived is not null)
        {
            await OnMessageRecived(this, new(topic, payload));
        }
    }

    public void Connect()
    {
        if (!_mqttClient.IsConnected)
            _mqttClient.ConnectAsync(_options).Wait();
    }

    public void Subscribe(string topic)
    {
        var topicFilter = new MqttTopicFilterBuilder()
            .WithTopic(topic)
            .Build();

        _mqttClient.SubscribeAsync(topicFilter).Wait();
    }
}
public delegate Task AsyncMessageHandler(object? sender, MqttObject e);