using MQTTnet;
using MQTTnet.Client;

namespace HikVisionConverter.Communication;

public interface IPTZMqttPublisher
{
    bool IsConnected { get; }

    Task Connect();
    Task<bool> Publish(PTZControl PTZ, string? payload = null);
    Task<bool> Publish<T>(T dto, string? payload = null) where T : BaseDto;
}

public class PTZMqttPublisher : IPTZMqttPublisher
{
    private IMqttClient _mqttClient;
    private MqttClientOptions _options;

    public bool IsConnected => _mqttClient.IsConnected;

    public PTZMqttPublisher()
    {
        MqttFactory factory = new();
        _mqttClient = factory.CreateMqttClient();
        _options = new MqttClientOptionsBuilder()
            .WithClientId(Guid.NewGuid().ToString())
            .WithTcpServer("127.0.0.1")
            .WithCleanSession()
            .Build();
    }
    public async Task Connect()
    {
        await _mqttClient.ConnectAsync(_options);
    }

    public async Task<bool> Publish<T>(T dto, string? payload = null) where T : BaseDto
    {
        var msg = new MqttApplicationMessageBuilder()
            .WithTopic(nameof(PTZControl))
            .WithPayload(dto.ToJsonString())
            .Build();

        if (_mqttClient.IsConnected)
        {
            await _mqttClient.PublishAsync(msg);
            return true;
        }
        else
        {
            Console.WriteLine("client is not connected");
            return false;
        }
    }

    public async Task<bool> Publish(PTZControl PTZ, string? payload = null)
    {
        var msg = new MqttApplicationMessageBuilder()
            .WithTopic(nameof(PTZControl))
            .WithPayload(PTZ.ToString())
            .Build();

        if (_mqttClient.IsConnected)
        {
            await _mqttClient.PublishAsync(msg);
            return true;
        }
        else
        {
            Console.WriteLine("client is not connected");
            return false;
        }
    }
}
