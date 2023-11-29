using System;

namespace HikVisionConverter.Communication;

/// <summary>
/// Navigate to Implamentation -> <see cref="CommandHandler"/>
/// </summary>
public interface ICommandHandler
{
    Task HandleCommand(CommandMessage request);
}

public class CommandHandler : ICommandHandler
{
    private readonly MarsRepository _repo;
    private readonly IPTZMqttPublisher _mqtt;

    public CommandHandler(MarsRepository repo,
        IPTZMqttPublisher mqtt)
    {
        _repo = repo;
        _mqtt = mqtt;
        _mqtt.Connect();
    }
    public async Task HandleCommand(CommandMessage request)
    {
        if (request is null) return;

        string clientName = request.RequestorIdentification;

        // Update last connection time
        if (_repo.MarsClients.ContainsKey(clientName))
            _repo.MarsClients[clientName].LastConnectionTime = DateTime.Now;

        var command = request.Command.Item;

        switch (command)
        {
            case SimpleCommandType simpleCommand:
                await HandleSimpleCommand(simpleCommand, clientName);
                break;
            case OpticalCommandType opticalCommand when request.SensorIdentification is not null:
                await HandleOpticalCommand(opticalCommand, request.SensorIdentification.SensorType);
                break;
            case LocationCommandType locationCommand:
                await HandleLocationCommand(locationCommand);
                break;
            case VideoSwitchCommandType videoSwitchCommand:
                await HandleVideoSwitch(videoSwitchCommand);
                break;
                //case ScriptCommandType scriptCommand:
                //    HandleScriptCommand(scriptCommand);
                //    break;
        }

    }

    private void HandleScriptCommand(ScriptCommandType command)
    {
        throw new NotImplementedException();
    }

    private async Task HandleVideoSwitch(VideoSwitchCommandType videoSwitchCommand)
    {
        if (videoSwitchCommand.SimpleCommand != SimpleCommandType.Set)
            return;

        var videoChannels = videoSwitchCommand.VideoChannel;
        if (videoChannels?.Length <= 0)
            return;

        foreach (var videoChannel in videoChannels)
        {
            if (videoChannel.Item is SensorIdentificationType)
            {
                var sensorIdentification = (SensorIdentificationType)videoChannel.Item;
                if (videoChannel.VideoChannelID == VideoChannelIDType.Item1)

                SetSensorInVideoChannel(videoChannel.VideoChannelID,
                    sensorIdentification.SensorType, sensorIdentification.SensorName);
                await _repo.SendSpecificFullStatusReportToAll(sensorIdentification.SensorType);
            }

            if (videoChannel.Item is SensorTypeType sensorType)
            {
                if (videoChannel.VideoChannelID == VideoChannelIDType.Item1)

                    SetSensorInVideoChannel(videoChannel.VideoChannelID, sensorType);
                await _repo.SendSpecificFullStatusReportToAll(sensorType);
            }
        }

        await _mqtt.Publish<VideoSwitchDto>(new VideoSwitchDto(PTZControl.DayMode),nameof(SensorTypeType.VideoSwitch));
    }

    private void SetSensorInVideoChannel(VideoChannelIDType id, SensorTypeType sensorType, string sensorName = null)
    {
        if (_repo.FullStatusReport?.Items == null)
            return;

        foreach (var item in _repo.FullStatusReport.Items)
        {
            if (item is SensorStatusReport sensorStatusReport)
            {
                if (sensorStatusReport.Item is VideoSwitchStatus)
                {
                    VideoSwitchStatus videoSwitchStatus = (VideoSwitchStatus)sensorStatusReport.Item;
                    foreach (var videoChannel in videoSwitchStatus.VideoChannel)
                    {
                        if (videoChannel.VideoChannelID == id)
                        {
                            if (sensorName != null)
                            {
                                videoChannel.Item = new SensorIdentificationType
                                {
                                    SensorSubTypeSpecified = false,
                                    SensorName = sensorName,
                                    SensorType = sensorType
                                };
                            }
                            else videoChannel.Item = sensorType;

                            return;
                        }
                    }
                }
            }
        }
    }

    private async Task HandleLocationCommand(LocationCommandType locationCommand)
    {
        switch (locationCommand.SimpleCommand)
        {
            case SimpleCommandType.Move:
            {
                if (locationCommand.Items.Length > 0)
                {
                    double marsHorizontalSpeed = locationCommand.Items[0].Value;
                    double marsVerticalSpeed = locationCommand.Items[1].Value;

                    await _mqtt.Publish<MovementDto>(new MovementDto(GetCameraMovingCommand(marsHorizontalSpeed, marsVerticalSpeed),
                    (float)marsHorizontalSpeed, (float)marsVerticalSpeed));
                }
                else await _mqtt.Publish<MovementDto>(new MovementDto(PTZControl.Stop,0,0));
                    return;
            }
            case SimpleCommandType.SetNorth:
            {
                if (locationCommand.Point.Length > 0 &&
                    locationCommand.Point[0].Item is RelativeLocationType)
                {
                    var relativeLocation = (RelativeLocationType)locationCommand.Point[0].Item;
                    SetNorth(relativeLocation);
                }
                return;
            }
        }
    }

    private async Task HandleOpticalCommand(OpticalCommandType opticalCommand, SensorTypeType sensor)
    {
        switch (opticalCommand.SimpleCommand)
        {
            case SimpleCommandType.Contrast:
                //do nothing
            case SimpleCommandType.Filter:
                //do nothing
            case SimpleCommandType.Zoom:
                switch (opticalCommand.Operation)
                {
                    case OperationType.Plus:
                        await _mqtt.Publish(PTZControl.ZoomIn);
                        return;
                    case OperationType.Minus:
                        await _mqtt.Publish(PTZControl.ZoomOut);
                        return;
                    case OperationType.Set:
                        //do nothing
                    default:
                        return;
                }
            case SimpleCommandType.Focus:
                switch (opticalCommand.Operation)
                {
                    // TODO: check if near is plus or minus
                    case OperationType.Plus:
                        //CommandReceived?.Invoke(PTZControl.FocusNear);
                        return;
                    case OperationType.Minus:
                        //CommandReceived?.Invoke(PTZControl.FocusFar);
                        return;
                    default:
                        //CommandReceived?.Invoke(PTZControl.FocusAuto);
                        return;
                }

            case SimpleCommandType.Gain:
                // CommandReceived?.Invoke(PTZControl.gain)
                return;

            case SimpleCommandType.Restart:
                await _mqtt.Publish(PTZControl.Restart);
                return;
        }
    }

    private async Task HandleSimpleCommand(SimpleCommandType simpleCommand, string deviceName)
    {
        switch (simpleCommand)
        {
            case SimpleCommandType.Stop:
                await _mqtt.Publish<MovementDto>(new MovementDto(PTZControl.Stop, 0, 0));
                return;
            case SimpleCommandType.Abort:
                //dunno
                return;
            case SimpleCommandType.Reset:
                await _mqtt.Publish(PTZControl.Reset);
                return;
            case SimpleCommandType.Restart:
                await _mqtt.Publish(PTZControl.Restart);
                return;
            case SimpleCommandType.KeepAlive:
                await _repo.SendEmptyDeviceStatusReport(deviceName);
                return;
            default:
                // Only keep alive commands are supported
                Console.WriteLine($"{deviceName} sent an unsupported command of type: {simpleCommand}");
                return;
        }
    }

    private PTZControl GetCameraMovingCommand(double horizontalSpeed, double verticalSpeed)
    {
        if (horizontalSpeed != 0 && verticalSpeed != 0)
        {
            if (verticalSpeed > 0)
                return horizontalSpeed > 0 ? PTZControl.RightUp : PTZControl.LeftUp;

            return horizontalSpeed > 0 ? PTZControl.RightDown : PTZControl.LeftDown;
        }

        if (verticalSpeed != 0)
            return verticalSpeed > 0 ? PTZControl.Up : PTZControl.Down;

        if (horizontalSpeed != 0)
            return horizontalSpeed > 0 ? PTZControl.Right : PTZControl.Left;

        return PTZControl.Stop;
    }

    private void SetNorth(RelativeLocationType relativeLocation)
    {
        if (relativeLocation.Azimuth == null || relativeLocation.Elevation == null)
            return;

        if (_repo.FullStatusReport?.Items == null)
            return;

        foreach (var item in _repo.FullStatusReport.Items)
        {
            if (item is SensorStatusReport)
            {
                var sensorStatusReport = (SensorStatusReport)item;
                if (sensorStatusReport.Item is PedestalStatus)
                {
                    var pedestalStatus = (PedestalStatus)sensorStatusReport.Item;
                    if (pedestalStatus.LastNorthingTime != null)
                        pedestalStatus.LastNorthingTime.Value = DateTime.Now;
                    else
                    {
                        pedestalStatus.LastNorthingTime = new TimeType
                        {
                            Zone = TimezoneType.GMT,
                            Value = DateTime.Now
                        };
                    }
                    break;
                }
            }
        }
    }

}
