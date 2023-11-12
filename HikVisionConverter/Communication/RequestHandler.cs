namespace HikVisionConverter.Communication;

/// <summary>
/// Navigate to Implamentation -> <see cref="RequestHandler"/>
/// </summary>
public interface IRequestHandler
{
    Task HandleConfigRequest(DeviceConfiguration request);
    Task HandleSubscriptionRequest(DeviceSubscriptionConfiguration request);
}

public class RequestHandler : IRequestHandler
{
    private readonly MarsRepository _repo;
    private readonly CycleTimerService _timerService;

    public RequestHandler(MarsRepository marsRepository,
        CycleTimerService timerService)
    {
        _repo = marsRepository;
        _timerService = timerService;

        _timerService.Start();
    }

    public async Task HandleConfigRequest(DeviceConfiguration request)
    {
        if (request is null)
            return;

        if (!request.MessageTypeSpecified || request.MessageType != MessageType.Request)
            return;

        string marsIp = request.NotificationServiceIPAddress;
        string marsPort = request.NotificationServicePort;
        string clientName = request.RequestorIdentification;


        MarsClient client = new()
        {
            IP = marsIp,
            Port = int.Parse(marsPort),
            LastConnectionTime = DateTime.Now,
        };

        client.Init(marsIp, marsPort);

        //might validate w/ schemaValidator

        _repo.MarsClients[clientName] = client;

        client.SoapClient!.ChannelFactory.Open();

        await client.SoapClient.doDeviceConfigurationAsync(ResponseMapper.Map(_repo.Configuration));
    }

    public async Task HandleSubscriptionRequest(DeviceSubscriptionConfiguration request)
    {
        if (request is null) return;

        string clientName = request.RequestorIdentification;

        SubscriptionTypeType[] subscribeTypes = request.SubscriptionType;

        if (_repo.MarsClients.ContainsKey(clientName))
        {
            // If unsub request
            if (subscribeTypes == null || subscribeTypes.Length == 0)
            {
                _repo.MarsClients.Remove(clientName);
                return;
            }
            else _repo.MarsClients[clientName].SubscriptionTypes = subscribeTypes;

            // Update connection watch to this mars
            _repo.MarsClients[clientName].LastConnectionTime = DateTime.Now;
        }
        else
        {
            Console.WriteLine("unknown Mars");
        }


        // lev gets camera positon...

        await _repo.SendFullStatusReport(clientName);
    }
}
