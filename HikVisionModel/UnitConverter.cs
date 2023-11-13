using System.Reflection.Metadata.Ecma335;

namespace HikVisionModel;

public static class UnitConverter
{
    const int MIN_MARS_SPEED = 0;
    const int MAX_MARS_SPEED = 6400;
    const float MIN_CAM_SPEED = 0f;
    const float MAX_CAM_SPEED = 1f;

    public static float MarsToCameraSpeed(int inputValue)
        => (float)(inputValue - MIN_MARS_SPEED) / (MAX_MARS_SPEED - MIN_MARS_SPEED) * (MAX_CAM_SPEED - MIN_CAM_SPEED) + MIN_CAM_SPEED;

    public static float MarsToCameraSpeed(string inputValue)
        => (float)(int.Parse(inputValue) - MIN_MARS_SPEED) / (MAX_MARS_SPEED - MIN_MARS_SPEED) * (MAX_CAM_SPEED - MIN_CAM_SPEED) + MIN_CAM_SPEED;

    public static float MarsToCameraSpeed(float inputValue)
        => (float)(inputValue - MIN_MARS_SPEED) / (MAX_MARS_SPEED - MIN_MARS_SPEED) * (MAX_CAM_SPEED - MIN_CAM_SPEED) + MIN_CAM_SPEED;

    public static float MarsToCameraVector(float inputValue)
        => inputValue >= 0 ? 1f : -1f;
}
