using System.Reflection.Metadata.Ecma335;

namespace HikVisionModel;

public static class UnitConverter
{
    const int MIN_MARS_SPEED = 0;
    const int MAX_MARS_SPEED = 10;
    const int MIN_MOVEMENT_SPEED = 4;
    const float MIN_CAM_SPEED = 0f;
    const float MAX_CAM_SPEED = 1f;

    public static float MarsToCameraSpeed(float inputValue)
    {
        var speed = Math.Min(inputValue, MIN_MOVEMENT_SPEED);
        return (float)(speed - MIN_MARS_SPEED) / (MAX_MARS_SPEED - MIN_MARS_SPEED) * (MAX_CAM_SPEED - MIN_CAM_SPEED) + MIN_CAM_SPEED;
    }

    public static float MarsToCameraVector(float inputValue)
        => inputValue >= 0 ? 1f : -1f;

    public static double ConvertToAngle(double inputValue)
    {
        var asin = Math.Asin(inputValue);
        var rad = (180 / Math.PI);
        var res = asin * rad;
        return res + 180;
    }

    public static double DegreeToMils(double degree)
    {
        return degree * (6400 / 360);
    }

    //From -1 to 1 value
    public static double ConvertToMils(double inputValue)
    {
        var res = ConvertToAngle(inputValue);
        return DegreeToMils(res);
    }
}
