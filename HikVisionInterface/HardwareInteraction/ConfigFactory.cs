using Newtonsoft.Json;
using System.Xml;

namespace HikVisionInterface.HardwareInteraction;

internal class ConfigFactory
{
    public readonly static string CONFIG_FILE_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "Config.json");

    public static Config Build()
    {
        string path = CONFIG_FILE_PATH;
        try
        {
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<Config>(json)!;
            }
            else
            {
                var config = new Config();
                var json = JsonConvert.SerializeObject(config);
                File.WriteAllText(path, json);

                if (config is null || !config.ValidConfig())
                {
                    SerilogLogger.ErrorLog("config is not fully set up");
                    throw new Exception("config is not fully set up");
                }

                return config;
            }
        }
        catch (Exception ex)
        {
            SerilogLogger.ErrorLog(ex.Message);
            throw;
        }
    }
}
