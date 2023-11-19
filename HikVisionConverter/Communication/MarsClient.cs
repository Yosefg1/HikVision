using System.ServiceModel;
using System.Text;
using System.Xml;

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
            BasicHttpBinding basicHttpBinding = GetBinding();
            this.SoapClient = new SNSR_STDSOAPPortClient(basicHttpBinding,address);

        }

        private static BasicHttpBinding GetBinding() => new()
        {
            Name = "SNSR_STDSOAPBinding",
            CloseTimeout = new TimeSpan(0, 1, 0),
            OpenTimeout = new TimeSpan(0, 1, 0),
            ReceiveTimeout = new TimeSpan(0, 10, 0),
            SendTimeout = new TimeSpan(0, 1, 0),
            AllowCookies = false,
            BypassProxyOnLocal = false,
            MaxBufferSize = 65536,
            MaxBufferPoolSize = 524288,
            MaxReceivedMessageSize = 65536,
            TextEncoding = Encoding.UTF8,
            TransferMode = TransferMode.Buffered,
            UseDefaultWebProxy = true,
            ReaderQuotas = new XmlDictionaryReaderQuotas
            {
                MaxDepth = 32,
                MaxStringContentLength = 8192,
                MaxArrayLength = 16384,
                MaxBytesPerRead = 4096,
                MaxNameTableCharCount = 16384
            },
            Security = new BasicHttpSecurity
            {
                Mode = BasicHttpSecurityMode.None,
                Transport = new HttpTransportSecurity
                {
                    ClientCredentialType = HttpClientCredentialType.None,
                    ProxyCredentialType = HttpProxyCredentialType.None
                }
            }
        };
    }
}
