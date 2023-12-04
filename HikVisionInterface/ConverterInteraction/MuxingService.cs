using HikVisionModel.Enums;

namespace HikVisionInterface.ConverterInteraction;

public class MuxingService
{
    private readonly CameraService _camera;
    private readonly FfmpegManager _ffmpegManager;

    private class DtoTest : BaseDto
    {
        // class used to check what DtoEnum
    }

    public MuxingService(CameraService camera, 
        FfmpegManager ffmpegManager)
    {
        _camera = camera;
        _ffmpegManager = ffmpegManager;
    }

    /// <summary>
    /// Converts json string into one of the dto types, and handles each according to its type...
    /// muxing the following types:
    /// <see cref="MovementDto"/>: calls camera service.
    /// <see cref="ZoomDto"/>: calls camera service.
    /// <see cref="PanTiltDto"/>: ignores.
    /// <see cref="VideoSwitchDto"/>: calls ffmpeg manager.
    /// </summary>
    /// <returns></returns>
    public async Task Mux(string json, string topic)
    {
        List<string> enumList = Enum.GetValues(typeof(PTZControl))
                                .Cast<PTZControl>()
                                .Select(x => x.ToString())
                                .ToList();

        var test = BaseDto.ToJsonObject<DtoTest>(json);
        DtoEnum dtoType = test.DtoId;

        PTZControl eTopic = (PTZControl)Enum.Parse(typeof(PTZControl), test.PTZ.ToString());

        SerilogLogger.ConsoleLog($"{topic}.{test.PTZ}");

        if (!enumList.Contains(test.PTZ.ToString())) return;

        switch (dtoType)
        {
            case DtoEnum.MovementDto:
                var mDto = BaseDto.ToJsonObject<MovementDto>(json);

                await _camera.MoveAsync(mDto);
                break;
            case DtoEnum.ZoomDto:
                var zDto = BaseDto.ToJsonObject<ZoomDto>(json);

                SerilogLogger.ConsoleLog($"{zDto.PTZ} + {zDto.Value}");
                await _camera.ZoomAsync(zDto);
                break;
            case DtoEnum.PanTiltDto:
                //ignore
                break;
            case DtoEnum.VideoSwitchDto:
                var vsDto = BaseDto.ToJsonObject<VideoSwitchDto>(json);
                if (vsDto.VideoSwitch && _ffmpegManager.IsSwitch is true)
                    _ffmpegManager.Switch();
                else if (!vsDto.VideoSwitch && _ffmpegManager.IsSwitch is false)
                    _ffmpegManager.Switch();
            break;
        }
    }
}
