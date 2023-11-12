using Newtonsoft.Json;
using System.Xml;

namespace HikVisionApiInterface.HardwareInteraction;

internal class ConfigFactory
{
    public readonly static string CONFIG_FILE_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", "Config.json");

    public static Config Build()
    {
        try
        {
            if (File.Exists(CONFIG_FILE_PATH))
            {
                var json = File.ReadAllText(CONFIG_FILE_PATH);
                return JsonConvert.DeserializeObject<Config>(json)!;
            }
            else
            {
                var config = new Config();
                var json = JsonConvert.SerializeObject(config);
                File.WriteAllText(CONFIG_FILE_PATH, json);
                return config;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}
