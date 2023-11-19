//using MQTTnet;
//using MQTTnet.Client;

//namespace HikVisionConverter.Communication;

//public interface IPTZMqttPublisher
//{
//    bool IsConnected { get; }

//    Task Connect();
//    Task<bool> Publish(PTZControl PTZ);
//    Task<bool> Publish<T>(T dto, string topic) where T : BaseDto;
//}

//public class PTZMqttPublisher : IPTZMqttPublisher
//{
//    private IMqttClient _mqttClient;
//    private MqttClientOptions _options;

//    public bool IsConnected => _mqttClient.IsConnected;

//    public PTZMqttPublisher()
//    {
//        MqttFactory factory = new();
//        _mqttClient = factory.CreateMqttClient();
//        _options = new MqttClientOptionsBuilder()
//            .WithClientId(Guid.NewGuid().ToString())
//            .WithTcpServer("127.0.0.1")
//            .WithCleanSession()
//            .Build();
//    }
//    public async Task Connect()
//    {
//        try
//        {
//            await _mqttClient.ConnectAsync(_options);

//        }
//        catch (Exception ex)
//        {
//            SerilogLogger.ErrorLog($"Error while trying to connect into mosquitto broker. try activating mosquitto on admin mode. \n{ex.Message}");
//        }
//    }

//    public async Task<bool> Publish<T>(T dto, string topic = nameof(PTZControl)) where T : BaseDto
//    {
//        var msg = new MqttApplicationMessageBuilder()
//            .WithTopic(topic)
//            .WithPayload(dto.ToJsonString())
//            .Build();

//        if (_mqttClient.IsConnected)
//        {
//            await _mqttClient.PublishAsync(msg);
//            return true;
//        }
//        else
//        {
//            SerilogLogger.ErrorLog("Error with publishing on mqtt, try activating mosquitto on admin mode.");
//            return false;
//        }
//    }

//    public async Task<bool> Publish(PTZControl PTZ)
//    {
//        var msg = new MqttApplicationMessageBuilder()
//            .WithTopic(nameof(PTZControl))
//            .WithPayload(PTZ.ToString())
//            .Build();

//        if (_mqttClient.IsConnected)
//        {
//            await _mqttClient.PublishAsync(msg);
//            return true;
//        }
//        else
//        {
//            SerilogLogger.ErrorLog("Error with publishing on mqtt, try activating mosquitto on admin mode.");
//            return false;
//        }
//    }
//}
