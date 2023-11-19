using System.Text.Json;
using System.Text.Json.Serialization;

namespace HikVisionConverter.Communication;

public class MarsRepository
{
    private readonly XmlFileManager _xml;
    private readonly PTZMqttSubscriber _subscriber;

    public MarsRepository(XmlFileManager xml,
        PTZMqttSubscriber subscriber)
    {
        _subscriber = subscriber;
        _subscriber.Connect();
        _subscriber.Subscribe(nameof(Topics.PanTiltStatus));
        _subscriber.OnMessageRecived += OnMqttMessageRecived;

        _xml = xml;
        MarsClients = new();
        Configuration = _xml.GetXml<DeviceConfiguration>(XmlType.Configuration);
        Configuration.NotificationServiceIPAddress = IP.GetLocalIPAddress();
        Configuration.NotificationServicePort = "5296";
        FullStatusReport = _xml.GetXml<DeviceStatusReport>(XmlType.FullStatus);
        //EmptyStatusReport = _xml.GetXml<DeviceStatusReport>(XmlType.EmptyStatus);
        EmptyStatusReport = CreateKeepAlive();
    }

    public Dictionary<string, MarsClient> MarsClients;

    public DeviceConfiguration Configuration { get; private set; }

    public DeviceStatusReport FullStatusReport { get; private set; }

    public DeviceStatusReport EmptyStatusReport { get; private set; }

    private async Task OnMqttMessageRecived(object? sender, MqttObject e)
    {
        if (e.Topic is not nameof(Topics.PanTiltStatus)) return;

        var pan = BaseDto.ToJsonObject<PanTiltDto>(e.Payload!).Pan;
        var tilt = BaseDto.ToJsonObject<PanTiltDto>(e.Payload!).Tilt;

        SerilogLogger.ConsoleLog($"PanTilt Status Updated pan:{pan} tilt:{tilt}");

        foreach (var item in FullStatusReport.Items)
        {
            SensorStatusReport? k = item as SensorStatusReport;
            if (k is null) continue;
            if (k.Item is PedestalStatus pedestal)
            {
                pedestal.Elevation.Value = double.Parse(pan);
                pedestal.Azimuth.Value = double.Parse(tilt);
            }
        }

        _xml.Write<DeviceStatusReport>(FullStatusReport);
        //hi
    }

    public async Task SendEmptyDeviceStatusReport(string clientName)
    {
        if (!MarsClients.ContainsKey(clientName)) return;

        await MarsClients[clientName].SoapClient!.doDeviceStatusReportAsync(ResponseMapper.Map(EmptyStatusReport));


    }

    public async Task SendFullStatusReport(string client)
    {
        if (!MarsClients.ContainsKey(client)) return;

        await MarsClients[client].SoapClient!.doDeviceStatusReportAsync(ResponseMapper.Map(FullStatusReport));

        SerilogLogger.ConsoleLog("FullStatusReport Sent.");
    }

    private DeviceStatusReport CreateKeepAlive()
    {
        return new DeviceStatusReport
        {
            DeviceIdentification = new DeviceIdentificationType
            {
                DeviceCategorySpecified = false,
                DeviceName = "HikeVisionCameraDevice",
                DeviceType = DeviceTypeType.Undefined
            },
            Items = new object[]
                {
                    new SensorStatusReport
                    {
                        SensorIdentification = new SensorIdentificationType
                        {
                            SensorSubTypeSpecified = false,
                            SensorName = "Undefined",
                            SensorType = SensorTypeType.Undefined
                        },
                        SensorTechnicalState = BITResultType.Undefined,
                        CommunicationState = BITResultType.Undefined,
                        PowerState = StatusType.Undefined,
                        SensorMode = SensorModeType.Undefined
                    }
                },
            MessageType = MessageType.Response,
            ProtocolVersion = ProtocolVersionType.Item22
        };
    }
}
