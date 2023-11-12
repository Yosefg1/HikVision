namespace HikVisionConverter.Communication;

public class MarsRepository
{
    private readonly XmlFileManager _xml;

    public MarsRepository(XmlFileManager xml)
    {
        _xml = xml;
        MarsClients = new();
        Configuration = _xml.GetXml<DeviceConfiguration>(XmlType.Configuration);
        FullStatusReport = _xml.GetXml<DeviceStatusReport>(XmlType.FullStatus);
        EmptyStatusReport = _xml.GetXml<DeviceStatusReport>(XmlType.EmptyStatus);
    }
    public Dictionary<string, MarsClient> MarsClients;

    public DeviceConfiguration Configuration { get; private set; }

    public DeviceStatusReport FullStatusReport { get; private set; }

    public DeviceStatusReport EmptyStatusReport { get; private set; }


    public async Task SendEmptyDeviceStatusReport(string clientName)
    {
        if (!MarsClients.ContainsKey(clientName)) return;

        await MarsClients[clientName].SoapClient!.doDeviceStatusReportAsync(ResponseMapper.Map(EmptyStatusReport));
    }

    public async Task SendFullStatusReport(string client)
    {
        if (!MarsClients.ContainsKey(client)) return;

        await MarsClients[client].SoapClient!.doDeviceStatusReportAsync(ResponseMapper.Map(FullStatusReport));
    }
}
