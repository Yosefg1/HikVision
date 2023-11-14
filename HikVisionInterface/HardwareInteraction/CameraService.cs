using HikVisionModel;
using Onvif.Core.Client;
using Onvif.Core.Client.Common;

namespace HikVisionInterface.HardwareInteraction;

public class CameraService
{
    private Config? _cameraConfig;
    private Camera? _camera;

    public CameraService()
    {
        try
        {
            _cameraConfig = ConfigFactory.Build();
            var pth = ConfigFactory.CONFIG_FILE_PATH;
            if (!_cameraConfig.ValidConfig()) throw new Exception($"Config file is not valid go to {pth} and fill in the information");

            var account = new Account(_cameraConfig.IP, _cameraConfig.UserName, _cameraConfig.Password);


            //_camera = Camera.Create(account, ex => SerilogLogger.ErrorLog("ONVIF EXCEPTION - " + ex.Message));

        }
        catch (Exception ex)
        {
            SerilogLogger.ConsoleLog("Empty config file!!! - " + ex.Message);
        }
    }

    public async Task<bool> MoveAsync(MovementDto dto)
    {
        PTZControl eTopic = (PTZControl)Enum.Parse(typeof(PTZControl), dto.PTZ.ToString());

        var Xspeed = UnitConverter.MarsToCameraSpeed(dto.HVel);
        var Yspeed = UnitConverter.MarsToCameraSpeed(dto.VVel);

        var Xvector = UnitConverter.MarsToCameraVector(dto.HVel);
        var Yvector = UnitConverter.MarsToCameraVector(dto.VVel);

        switch (eTopic)
        {
            case PTZControl.Left:
                var leftVector = new PTZVector { PanTilt = new Vector2D { x = Xvector } };
                var leftSpeed = new PTZSpeed { PanTilt = new Vector2D { x = Xspeed } };
                return await _camera.MoveAsync(MoveType.Continuous, leftVector, leftSpeed, 0);
            case PTZControl.Right:
                var rightVector = new PTZVector { PanTilt = new Vector2D { x = Xvector } };
                var rightSpeed = new PTZSpeed { PanTilt = new Vector2D { x = Xspeed } };
                return await _camera.MoveAsync(MoveType.Continuous, rightVector, rightSpeed, 0);
            case PTZControl.Up:
                var upVector = new PTZVector { PanTilt = new Vector2D { y = Yvector } };
                var upSpeed = new PTZSpeed { PanTilt = new Vector2D { x = Xspeed, y = Yspeed } };
                return await _camera.MoveAsync(MoveType.Relative, upVector, upSpeed, 0);
            case PTZControl.Down:
                var downVector = new PTZVector { PanTilt = new Vector2D { y = Yvector } };
                var downSpeed = new PTZSpeed { PanTilt = new Vector2D { x = Xspeed, y = Yspeed } };
                return await _camera.MoveAsync(MoveType.Relative, downVector, downSpeed, 0);
            case PTZControl.LeftUp:
                var leftUpVector = new PTZVector { PanTilt = new Vector2D { x = Xvector, y = Yvector } };
                var leftUpSpeed = new PTZSpeed { PanTilt = new Vector2D { x = Xspeed, y = Yspeed } };
                return await _camera.MoveAsync(MoveType.Continuous, leftUpVector, leftUpSpeed, 0);
            case PTZControl.RightUp:
                var rightUpVector = new PTZVector { PanTilt = new Vector2D { x = Xvector, y = Yvector } };
                var rightUpSpeed = new PTZSpeed { PanTilt = new Vector2D { x = Xspeed   , y = Yspeed } };
                return await _camera.MoveAsync(MoveType.Continuous, rightUpVector, rightUpSpeed, 0);
            case PTZControl.LeftDown:
                var leftDownVector = new PTZVector { PanTilt = new Vector2D { x = Xvector, y = Yvector } };
                var leftDownSpeed = new PTZSpeed { PanTilt = new Vector2D { x = Xspeed, y = Yspeed } };
                return await _camera.MoveAsync(MoveType.Continuous, leftDownVector, leftDownSpeed, 0);
            case PTZControl.RightDown:
                var rightDownVector = new PTZVector { PanTilt = new Vector2D { x = Xvector, y = Yvector } };
                var rightDownSpeed = new PTZSpeed { PanTilt = new Vector2D { x = Xspeed, y = Yspeed } };
                return await _camera.MoveAsync(MoveType.Continuous, rightDownVector, rightDownSpeed, 0);
            case PTZControl.ZoomIn:
                var zoomInVector = new PTZVector { Zoom = new Vector1D { x = Xspeed } };
                var zoomInSpeed = new PTZSpeed { Zoom = new Vector1D { x = Xspeed } };
                return await _camera.MoveAsync(MoveType.Relative, zoomInVector, zoomInSpeed, 0);
            case PTZControl.ZoomOut:
                var zoomOutVector = new PTZVector { Zoom = new Vector1D { x = Xspeed } };
                var zoomOutSpeed = new PTZSpeed { Zoom = new Vector1D { x = Xspeed } };
                return await _camera.MoveAsync(MoveType.Relative, zoomOutVector, zoomOutSpeed, 0);
            case PTZControl.Stop:
            var stopVector = new PTZVector { PanTilt = new Vector2D { x = 0f, y = 0f }, Zoom = new Vector1D { x = 0f } };
            var stopSpeed = new PTZSpeed { PanTilt = new Vector2D { x = 0f, y = 0f }, Zoom = new Vector1D { x = 0f } };
            return await _camera.MoveAsync(MoveType.Relative, stopVector, stopSpeed, 0);
        case PTZControl.Reset:
            return true;
        case PTZControl.Restart:
            return true;
        default: 
            return true;


        }
    }
}
