using System.IO;
using System.Web.Services.Description;
using System.Xml.Serialization;

namespace HikVisionConverter.Communication;

public enum XmlType
{
    Configuration,
    FullStatus,
    EmptyStatus
}
public class XmlFileManager
{

    private readonly Dictionary<XmlType, string> _dict = new()
    {
        { XmlType.Configuration, "DeviceConfiguration.xml" },
        { XmlType.FullStatus, "FullStatusReport.xml" },
        { XmlType.EmptyStatus, "EmptyStatusReport.xml" }
    };

    public T GetXml<T>(XmlType fileType)
    {
        if (typeof(T) == typeof(DeviceConfiguration) && fileType is not XmlType.Configuration)
            throw new Exception("Programmer exception, please access your use of XmlFileManager.GetXml() and fix it...");
        if (typeof(T) == typeof(DeviceStatusReport) && fileType is XmlType.Configuration)
            throw new Exception("Programmer exception, please access your use of XmlFileManager.GetXml() and fix it...");
        try
        {
            T deviceMsg;

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", _dict[fileType]);

            XmlSerializer serializer = new(typeof(T));
            using (FileStream fileStream = new(filePath, FileMode.Open))
            {
                deviceMsg = (T)serializer.Deserialize(fileStream)!;
            }

            return deviceMsg;
        }
        catch (Exception ex)
        {
            throw new Exception($"Problam getting {fileType} file \n {ex.Message}");
        }
    }
}
