﻿using Newtonsoft.Json;
using Onvif.Core.Client;
using Onvif.Core.Client.Common;

namespace HikVisionInterface.HardwareInteraction;

public class CameraService
{
    private readonly IPTZMqttPublisher _publisher;

    private HikVisionInterface.HardwareInteraction.Model.Config? _cameraConfig;
    private Camera? _camera;


    public CameraService(IPTZMqttPublisher publisher)
    {
        _publisher = publisher;
        _publisher.Connect();

        try
        {
            _cameraConfig = ConfigFactory.Build();
            var pth = ConfigFactory.CONFIG_FILE_PATH;
            if (!_cameraConfig.ValidConfig()) throw new Exception($"Config file is not valid go to {pth} and fill in the information");

            var account = new Account(_cameraConfig.CameraIp, _cameraConfig.UserName, _cameraConfig.Password);

            _camera = Camera.Create(account, ex => SerilogLogger.ErrorLog("ONVIF EXCEPTION - " + ex.Message));
        }
        catch (Exception ex)
        {
            SerilogLogger.ConsoleLog("Empty config file!!! - " + ex.Message);
        }
    }

    public async Task<PanTiltDto> GetPanTiltAsync()
    {
        try
        {
            var status = await _camera!.Ptz.GetStatusAsync(_camera.Profile.token);

            SerilogLogger.ConsoleLog(JsonConvert.SerializeObject(status));

            var vector = status.Position.PanTilt;

            return new(vector.x.ToString(), vector.y.ToString());
        }
        catch (Exception ex)
        {
            SerilogLogger.ErrorLog($"Camera not connected, Sending dummy values \n{ex}");
            return new PanTiltDto("0.577", "0.3213");
        }
    }

    public async Task<bool> ZoomAsync(ZoomDto dto)
    {
        PTZControl eTopic = dto.PTZ;

        var Xspeed = UnitConverter.MarsToCameraSpeed(dto.Value);

        switch (eTopic)
        {
            case PTZControl.ZoomIn:
                var zoomInVector = new PTZVector { Zoom = new Vector1D { x = UnitConverter.MarsToCameraZoom(dto.Value) } };
                var zoomInSpeed = new PTZSpeed { Zoom = new Vector1D { x = Xspeed } };
                return await _camera.MoveAsync(MoveType.Relative, zoomInVector, zoomInSpeed, 0);
            case PTZControl.ZoomOut:
                var zoomOutVector = new PTZVector { Zoom = new Vector1D { x = -UnitConverter.MarsToCameraZoom(dto.Value) } };
                var zoomOutSpeed = new PTZSpeed { Zoom = new Vector1D { x = Xspeed } };
                return await _camera.MoveAsync(MoveType.Relative, zoomOutVector, zoomOutSpeed, 0);
            default: 
                return false;
        }

    }

    public async Task<bool> MoveAsync(MovementDto dto)
    {
        PTZControl eTopic = dto.PTZ;

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
            //case PTZControl.ZoomIn:
            //    var zoomInVector = new PTZVector { Zoom = new Vector1D { x = UnitConverter.MarsToCameraZoom(dto.HVel) } };
            //    var zoomInSpeed = new PTZSpeed { Zoom = new Vector1D { x = Xvector } };
            //    return await _camera.MoveAsync(MoveType.Relative, zoomInVector, zoomInSpeed, 0);
            //case PTZControl.ZoomOut:
            //    var zoomOutVector = new PTZVector { Zoom = new Vector1D { x =  -UnitConverter.MarsToCameraZoom(dto.HVel) } };
            //    var zoomOutSpeed = new PTZSpeed { Zoom = new Vector1D { x = Xspeed } };
            //    return await _camera.MoveAsync(MoveType.Relative, zoomOutVector, zoomOutSpeed, 0);
            case PTZControl.Stop:
                var stopVector = new PTZVector { PanTilt = new Vector2D { x = 0f, y = 0f }, Zoom = new Vector1D { x = 0f } };
                var stopSpeed = new PTZSpeed { PanTilt = new Vector2D { x = 0f, y = 0f }, Zoom = new Vector1D { x = 0f } };
                bool res = await _camera.MoveAsync(MoveType.Continuous, stopVector, stopSpeed, 0);

                var panTiltDto = await GetPanTiltAsync();
                await _publisher.Publish(panTiltDto, nameof(Topics.PanTiltStatus));
                return res;
            case PTZControl.Reset:
                return true;
            case PTZControl.Restart:
                return true;
            default: 
                return true;


        }
    }
}
