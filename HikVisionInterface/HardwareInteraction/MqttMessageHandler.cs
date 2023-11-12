using Microsoft.Extensions.Primitives;
using Onvif;
using Onvif.Core.Client;
using Onvif.Core.Client.Common;
using Serilog;

namespace HikVisionApiInterface.HardwareInteraction;

public class MqttMessageHandler
{
    private const float Speed = 0.5f;
    private Config? _cameraConfig;
    private Camera? _camera;

    public MqttMessageHandler()
    {
        try
        {
            _cameraConfig = ConfigFactory.Build();
            var pth = ConfigFactory.CONFIG_FILE_PATH;
            if (!_cameraConfig.ValidConfig()) throw new Exception($"Config file is not valid go to {pth} and fill in the information");

            var account = new Account(_cameraConfig.IP, _cameraConfig.UserName, _cameraConfig.Password);


            _camera = Camera.Create(account, ex => Log.Information("ONVIF EXCEPTION - " + ex.Message));

        }
        catch (Exception ex)
        {
            Log.Information("Empty config file!!! - " + ex.Message);
        }
    }

    public async Task HandleMqttMessage(string topic, string payload)
    {
        //if (Enum.TryParse<PTZControl>(payload, out PTZControl eTopic)) return;
        List<string> enumList = Enum.GetValues(typeof(PTZControl))
                                .Cast<PTZControl>()
                                .Select(x => x.ToString())
                                .ToList();
        if (!enumList.Contains(payload)) return;

        PTZControl eTopic = (PTZControl)Enum.Parse(typeof(PTZControl), payload);

        //Log.Information($"{eTopic}");
        Log.Information($"{topic}.{payload}");
        switch (eTopic)
        {
            case PTZControl.Left:
                var leftVector = new PTZVector { PanTilt = new Vector2D { x = 1f } };
                var leftSpeed = new PTZSpeed { PanTilt = new Vector2D { x = -1f } };
                await _camera.MoveAsync(MoveType.Continuous, leftVector, leftSpeed, 0);
                return;
            case PTZControl.Right:
                var rightVector = new PTZVector { PanTilt = new Vector2D { x = 1f } };
                var rightSpeed = new PTZSpeed { PanTilt = new Vector2D { x = 1f } };
                await _camera.MoveAsync(MoveType.Continuous, rightVector, rightSpeed, 0);
                return;
            case PTZControl.Up:
                var upVector = new PTZVector { PanTilt = new Vector2D { y = 1f } };
                var upSpeed = new PTZSpeed { PanTilt = new Vector2D { x = 1f, y = 1f } };
                await _camera.MoveAsync(MoveType.Relative, upVector, upSpeed, 0);
                return;
            case PTZControl.Down:
                var downVector = new PTZVector { PanTilt = new Vector2D { y = -1f } };
                var downSpeed = new PTZSpeed { PanTilt = new Vector2D { x = 1f, y = 1f } };
                await _camera.MoveAsync(MoveType.Relative, downVector, downSpeed, 0);
                return;
            case PTZControl.LeftUp:
                var leftUpVector = new PTZVector { PanTilt = new Vector2D { x = 1f, y = 1f } };
                var leftUpSpeed = new PTZSpeed { PanTilt = new Vector2D { x = -1f, y = 1f } };
                await _camera.MoveAsync(MoveType.Continuous, leftUpVector, leftUpSpeed, 0);
                return;
            case PTZControl.RightUp:
                var rightUpVector = new PTZVector { PanTilt = new Vector2D { x = 1f, y = 1f } };
                var rightUpSpeed = new PTZSpeed { PanTilt = new Vector2D { x = 1f, y = 1f } };
                await _camera.MoveAsync(MoveType.Continuous, rightUpVector, rightUpSpeed, 0);
                return;
            case PTZControl.LeftDown:
                var leftDownVector = new PTZVector { PanTilt = new Vector2D { x = 1f, y = 1f } };
                var leftDownSpeed = new PTZSpeed { PanTilt = new Vector2D { x = -1f, y = -1f } };
                await _camera.MoveAsync(MoveType.Continuous, leftDownVector, leftDownSpeed, 0);
                return;
            case PTZControl.RightDown:
                var rightDownVector = new PTZVector { PanTilt = new Vector2D { x = 1f, y = 1f } };
                var rightDownSpeed = new PTZSpeed { PanTilt = new Vector2D { x = 1f, y = -1f } };
                await _camera.MoveAsync(MoveType.Continuous, rightDownVector, rightDownSpeed, 0);
                return;
            case PTZControl.ZoomIn:
                var zoomInVector = new PTZVector { Zoom = new Vector1D { x = 1f } };
                var zoomInSpeed = new PTZSpeed { Zoom = new Vector1D { x = 1f } };
                await _camera.MoveAsync(MoveType.Relative, zoomInVector, zoomInSpeed, 0);
                return;
            case PTZControl.ZoomOut:
                var zoomOutVector = new PTZVector { Zoom = new Vector1D { x = -1f } };
                var zoomOutSpeed = new PTZSpeed { Zoom = new Vector1D { x = 1f } };
                await _camera.MoveAsync(MoveType.Relative, zoomOutVector, zoomOutSpeed, 0);
                return;
            case PTZControl.Stop:
                var stopVector = new PTZVector { PanTilt = new Vector2D { x = 0f, y = 0f }, Zoom = new Vector1D { x = 0f } };
                var stopSpeed = new PTZSpeed { PanTilt = new Vector2D { x = 0f, y = 0f }, Zoom = new Vector1D { x = 0f } };
                await _camera.MoveAsync(MoveType.Relative, stopVector, stopSpeed, 0);
                return;
            case PTZControl.Reset:
                return;
            case PTZControl.Restart:
                return;

        }
    }
}
