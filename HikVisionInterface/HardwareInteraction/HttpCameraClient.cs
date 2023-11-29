using Newtonsoft.Json;
using System.Text;
using static System.Net.WebRequestMethods;

namespace HikVisionInterface.HardwareInteraction;

public class HttpCameraClient
{
    private const string CAMERA_HTTP_URL = "http://94.97.2.40";
    private const string TOKEN = "64243c431304d472b578a2ad";

    static async Task SwitchVideo(int channelId)
    {
        // URL of the server
        string url = $"{CAMERA_HTTP_URL}/cgi-bin/proc.cgi";

        // Create the payload object
        var payloadObject = new
        {
            cmd = "multicastSet",
            param = new
            {
                token = TOKEN,
                channelid = channelId,
                streamid = 0,
                bEnable = 1,
                multicastAddrVideo = "224.0.0.100",
                multicastPortVideo = 16006,
                protocol = 0
            }
        };

        // Convert the payload object to a JSON string
        string payload = JsonConvert.SerializeObject(payloadObject);

        // Create the HttpClient instance
        using HttpClient client = new();
        // Create the content with the payload
        StringContent content = new(payload, Encoding.UTF8, "application/json");

        // Set additional headers
        content.Headers.Add("Host", "94.97.2.40");
        content.Headers.Add("Content-Length", content.ReadAsStringAsync().Result.Length.ToString());
        //content.Headers.Add("Origin", "http://94.97.2.40");
        //content.Headers.Add("Referer", "http://94.97.2.40/cfg/network_senior_set.html");

        SerilogLogger.ConsoleLog($"Sending SwitchVideo Http Command to {CAMERA_HTTP_URL}");
        HttpResponseMessage response = await client.PostAsync(url, content);

        string responseBody = await response.Content.ReadAsStringAsync();
    }
}
