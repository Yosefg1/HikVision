using HikVisionFfmpeg;
using Newtonsoft.Json;
using System.Xml;

namespace HikVisionFfmpeg;

internal class ConfigFactory
{
    public readonly static string CONFIG_FILE_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.json");

    public static FfmpegConfig Build()
    {
        try
        {
            if (File.Exists(CONFIG_FILE_PATH))
            {
                var json = File.ReadAllText(CONFIG_FILE_PATH);
                return JsonConvert.DeserializeObject<FfmpegConfig>(json)!;
            }
            else
            {
                var config = new FfmpegConfig();
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

