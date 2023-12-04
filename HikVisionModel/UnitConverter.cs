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
        return (float)(speed) / (MAX_MARS_SPEED) * MAX_CAM_SPEED;
    }

    static double ConvertTo360Angle(double value)
    {
        // Ensure the value is within the valid range [-1, 1]
        value = Math.Max(-1, Math.Min(1, value));

        // Convert to angle in radians
        double angleInRadians = Math.Acos(value);

        // Convert to angle in degrees
        double angleInDegrees = angleInRadians * (180 / Math.PI);

        // Adjust to cover the full 360-degree range
        double adjustedAngle = angleInDegrees * 2;

        return adjustedAngle;
    }

    public static float MarsToVerticalCameraSpeed(float input)
    {
        bool isNeg = false;
        if (input < 0) isNeg = true;
        if(input > MAX_MARS_SPEED) input = MAX_MARS_SPEED;
        if(input < MIN_MOVEMENT_SPEED) input = MIN_MOVEMENT_SPEED;
        if(isNeg) return ((float)(input) / (MAX_MARS_SPEED) * MAX_CAM_SPEED) * -1;
        return (float)(input) / (MAX_MARS_SPEED) * MAX_CAM_SPEED;
    }

    public static float MarsToCameraVector(float inputValue)
        => inputValue >= 0 ? 1f : -1f;

    static double MapToAngle(double value)
    {
        double minValue = -1;
        double maxValue = 1;
        double angleMin = -90;
        double angleMax = 90;

        // Ensure the value is within the valid range [-0.6, 0.6]
        value = Math.Max(minValue, Math.Min(maxValue, value));

        // Perform linear mapping to the angle range [-90, 90]
        double mappedAngle = (value - minValue) / (maxValue - minValue) * (angleMax - angleMin) + angleMin;

        return mappedAngle;
    }

    public static double DegreeToMils(double degree)
    {
        return degree * (6400 / 360);
    }

    public static double ConvertToMilsElevation(double input)
    {
        var res = MapToAngle(input);
        return DegreeToMils(res);
    }

    //From -1 to 1 value
    public static double ConvertToMilsAzimuth(double inputValue)
    {
        var res = ConvertTo360Angle(inputValue);
        return DegreeToMils(res);
    }

    public static float MarsToCameraZoom(float value)
    {
        return value / 100;
    }
}