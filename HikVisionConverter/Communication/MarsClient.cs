using System.ServiceModel;

namespace HikVisionConverter.Communication
{
    public class MarsClient
    {
        public SNSR_STDSOAPPortClient? SoapClient { get; private set; }

        public SubscriptionTypeType[]? SubscriptionTypes { get; set; }

        public DateTime LastConnectionTime { get; set; }

        public string? IP { get; set; }

        public int? Port { get; set; }

        public void Init(string marsIp, string marsPort)
        {
            EndpointAddress address = new($"http://{marsIp}:{marsPort}/SNSR_STD-SOAP");
            BasicHttpBinding basicHttpBinding = new();
            this.SoapClient = new SNSR_STDSOAPPortClient(basicHttpBinding,address);
        }
    }
}
