using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace HikVisionConverter.Communication;

public class MarsRepository
{
    private readonly XmlFileManager _xml;
    private readonly PTZMqttSubscriber _subscriber;

    public MarsRepository(XmlFileManager xml,
        PTZMqttSubscriber subscriber)
    {
        IsDayCamera = true;

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

    public bool IsDayCamera { get; private set; }

    public Dictionary<string, MarsClient> MarsClients;

    public DeviceConfiguration Configuration { get; private set; }

    public DeviceStatusReport FullStatusReport { get; private set; }

    public DeviceStatusReport EmptyStatusReport { get; private set; }

    public void SwitchCamera()
    {
        IsDayCamera = !IsDayCamera;
    }

    private async Task OnMqttMessageRecived(object? sender, MqttObject e)
    {
        if (e.Topic is not nameof(Topics.PanTiltStatus)) return;


        var obj = BaseDto.ToJsonObject<PanTiltDto>(e.Payload!);

        SerilogLogger.ConsoleLog(e.Payload!);
        var pan = obj.Pan;
        var tilt = obj.Tilt;


        foreach (var item in FullStatusReport.Items)
        {
            if (item is not SensorStatusReport k) continue;

            if (k.Item is PedestalStatus pedestal)
            {
                var elevation = UnitConverter.ConvertToMilsElevation(double.Parse(tilt));
                var azimuth = UnitConverter.ConvertToMilsAzimuth(double.Parse(pan));
                SerilogLogger.ConsoleLog($"elevation: {elevation * 360 / 6400} azimuth: {azimuth * 360 / 6400}");

                //if azimuth or eleveation is 0 mars thinks camera is לא זמין
                pedestal.Elevation.Value = elevation;
                pedestal.Azimuth.Value = azimuth;
            }
        }
        await SendFullStatusReportToAll();
        
        _xml.Write<DeviceStatusReport>(FullStatusReport);
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

        SerilogLogger.ConsoleLog($"FullStatusReport Sent to {client}.");
    }

    public async Task SendFullStatusReportToAll()
    {
        if (MarsClients.IsNullOrEmpty()) return;

        foreach (var mars in MarsClients)
        {
            await mars.Value.SoapClient!.doDeviceStatusReportAsync(ResponseMapper.Map(FullStatusReport));

            SerilogLogger.ConsoleLog($"FullStatusReport Sent to {mars.Key}.");

        }
    }

    public async Task SendSpecificFullStatusReportToAll(SensorTypeType sensorType)
    {
        var deviceStatusReport = CreateUpdatedStatusReportOfSpecificSensor(sensorType);


        foreach (string name in MarsClients.Keys)
        {
            // if client has subscribed to status reports
            if (MarsClients[name].SubscriptionTypes!.Contains(SubscriptionTypeType.TechnicalStatus))
            {
                await MarsClients[name].SoapClient!.doDeviceStatusReportAsync(ResponseMapper.Map(deviceStatusReport));
            }
        }

        await SendFullStatusReportToAll();

    }

    private DeviceStatusReport CreateUpdatedStatusReportOfSpecificSensor(SensorTypeType sensorType)
    {

        DeviceStatusReport deviceStatusReport = new()
        {
            DeviceIdentification = FullStatusReport.DeviceIdentification,
            MessageType = FullStatusReport.MessageType,
            ProtocolVersion = FullStatusReport.ProtocolVersion
        };

        List<object> itemsList = new List<object>();
        foreach (var item in FullStatusReport.Items)
        {
            if (item is SensorStatusReport)
            {
                var sensorStatusReport = (SensorStatusReport)item;
                var sensorStatusReportClone = new SensorStatusReport
                {
                    SensorIdentification = sensorStatusReport.SensorIdentification,
                    SensorTechnicalState = sensorStatusReport.SensorTechnicalState,
                    CommunicationState = sensorStatusReport.CommunicationState,
                    PowerState = sensorStatusReport.PowerState,
                    SensorMode = sensorStatusReport.SensorMode,
                    CalibrationState = sensorStatusReport.CalibrationState
                };

                if (sensorStatusReport.SensorIdentification?.SensorType == sensorType)
                    sensorStatusReportClone.Item = sensorStatusReport.Item;

                itemsList.Add(sensorStatusReport);
            }

            else if (item is DetailedSensorBITType)
                itemsList.Add((DetailedSensorBITType)item);
        }

        deviceStatusReport.Items = itemsList.ToArray();

        return deviceStatusReport;
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
