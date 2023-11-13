using HikVisionModel;

namespace HikVisionConverter.Communication;

public class MarsService : SNSR_STDSOAPPort
{
    private readonly IRequestHandler _requestHandler;
    private readonly ICommandHandler _commandHandler;

    public MarsService(IRequestHandler requestHandler, 
        ICommandHandler commandHandler)
    {
        _requestHandler = requestHandler;
        _commandHandler = commandHandler;
    }

    public async Task<doDeviceConfigurationResponse> doDeviceConfigurationAsync(doDeviceConfigurationRequest request)
    {
        await _requestHandler.HandleConfigRequest(request.DeviceConfiguration);
        SerilogLogger.ConsoleLog($"Device Configuration sent to {request.DeviceConfiguration.RequestorIdentification}" +
            $" with ip {request.DeviceConfiguration.NotificationServiceIPAddress}:{request.DeviceConfiguration.NotificationServicePort}");
        return new doDeviceConfigurationResponse();
    }

    public async Task<doDeviceSubscriptionConfigurationResponse> doDeviceSubscriptionConfigurationAsync(doDeviceSubscriptionConfigurationRequest request)
    {
        await _requestHandler.HandleSubscriptionRequest(request.DeviceSubscriptionConfiguration);
        return new doDeviceSubscriptionConfigurationResponse();
    }


    public async Task<doCommandMessageResponse> doCommandMessageAsync(doCommandMessageRequest request)
    {
        await _commandHandler.HandleCommand(request.CommandMessage);
        return new doCommandMessageResponse();
    }

    public Task<doCommandMessageResponse>? doDeviceStatusReportAsync(doDeviceStatusReportRequest request)
    {
        //Status happends in Mars not in the Sensor
        return null;
    }

    public Task<doCommandMessageResponse>? doDeviceIndicationReportAsync(doDeviceIndicationReportRequest request)
    {
        //Status happends in Mars not in the Sensor
        return null;
    }


}
